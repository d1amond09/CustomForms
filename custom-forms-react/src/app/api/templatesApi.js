import { apiSlice } from './apiSlice';

export const templatesApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        getTemplates: builder.query({
            
            query: (params) => ({
                url: '/templates',
                params: params, 
            }),
            transformResponse: (response, meta, arg) => {
                const paginationHeader = meta?.response?.headers?.get('X-Pagination');
                let paginationMetaData = null;

                if (paginationHeader) {
                    try {
                        paginationMetaData = JSON.parse(paginationHeader);
                    } catch (e) {
                        console.error("Failed to parse X-Pagination header:", e);
                        paginationMetaData = { currentPage: 1, totalPages: 1, pageSize: arg.pageSize, totalCount: response?.length ?? 0 };
                    }
                } else {
                    console.warn("X-Pagination header not found in response.");
                    paginationMetaData = { currentPage: arg.pageNumber, totalPages: arg.pageNumber, pageSize: arg.pageSize, totalCount: response?.length ?? 0 };
                }


                return {
                    items: response,
                    metaData: paginationMetaData
                };
            },
            providesTags: (result, error, arg) =>
                result?.items 
                    ? [
                        ...result.items.map(({ id }) => ({ type: 'Template', id })),
                        { type: 'Template', id: 'LIST' }, 
                    ]
                    : [{ type: 'Template', id: 'LIST' }],
        }),


        getTemplateById: builder.query({
            query: (id) => `/templates/${id}`,
            providesTags: (result, error, id) => {
                let tags = [];
                if (result) { 
                    tags.push({ type: 'Template', id });
                    if (Array.isArray(result.comments)) {
                        tags = tags.concat(result.comments.map(c => ({ type: 'Comment', id: c.id })));
                    } else {
                        console.warn("result.comments is not an array in providesTags for getTemplateById:", result?.comments);
                    }
                    return tags;
                } else {
                    return id ? [{ type: 'Template', id }] : [];
                }
            }
        }),


        getLatestPublicTemplates: builder.query({
            query: (count = 5) => `/templates/latest?count=${count}`,
            providesTags: (result, error, arg) => [{ type: 'Template', id: 'LATEST_LIST' }], 
        }),

        getPopularPublicTemplates: builder.query({
            query: (count = 5) => `/templates/popular?count=${count}`,
            providesTags: (result, error, arg) => [{ type: 'Template', id: 'POPULAR_LIST' }], 
        }),

        createTemplate: builder.mutation({
            query: (templateData) => ({ 
                url: '/templates',
                method: 'POST',
                body: templateData,
            }),
            invalidatesTags: [{ type: 'Template', id: 'LIST' }, { type: 'Template', id: 'LATEST_LIST' }], 
        }),

        updateTemplate: builder.mutation({
            query: ({ id, templateData }) => ({ 
                url: `/templates/${id}`,
                method: 'PUT',
                body: templateData,
            }),
            invalidatesTags: (result, error, { id }) => [{ type: 'Template', id }, { type: 'Template', id: 'LIST' }],
        }),

        deleteTemplate: builder.mutation({
            query: (id) => ({
                url: `/templates/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: (result, error, id) => [{ type: 'Template', id }, { type: 'Template', id: 'LIST' }, { type: 'Template', id: 'LATEST_LIST' }, { type: 'Template', id: 'POPULAR_LIST' }],
        }),

        addQuestion: builder.mutation({
            query: ({ templateId, questionData }) => ({ 
                url: `/templates/${templateId}/questions`,
                method: 'POST',
                body: questionData,
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
        }),

        removeQuestion: builder.mutation({
            query: ({ templateId, questionId }) => ({
                url: `/templates/${templateId}/questions/${questionId}`,
                method: 'DELETE',
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
        }),

        updateQuestion: builder.mutation({
            query: ({ templateId, questionId, questionData }) => ({ 
                url: `/templates/${templateId}/questions/${questionId}`,
                method: 'PUT',
                body: questionData,
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
        }),

        reorderQuestions: builder.mutation({
            query: ({ templateId, reorderData }) => ({
                url: `/templates/${templateId}/questions/reorder`,
                method: 'PUT',
                body: reorderData,
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
        }),


        setTags: builder.mutation({
            query: ({ templateId, tagNames }) => ({ 
                url: `/templates/${templateId}/tags`,
                method: 'PUT',
                body: tagNames,
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }, { type: 'Tag', id: 'LIST' }, { type: 'Tag', id: 'POPULAR_LIST' }], 
        }),


        setAccess: builder.mutation({
            query: ({ templateId, accessData }) => ({ 
                url: `/templates/${templateId}/access`,
                method: 'PUT',
                body: accessData,
            }),
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
        }),


        toggleLike: builder.mutation({
            query: (templateId) => ({
                url: `/likes/toggle/${templateId}`, 
                method: 'POST',
            }),

            async onQueryStarted(templateId, { dispatch, queryFulfilled, getState }) {
                const patchResult = dispatch(
                    templatesApi.util.updateQueryData('getTemplateById', templateId, (draft) => {
                        const userId = getState().auth.user?.id;
                        if (!userId) return; 

                        const likeIndex = draft.likes?.findIndex(l => l.userId === userId) ?? -1;

                        if (likeIndex > -1) {
                            if (draft.likes) draft.likes.splice(likeIndex, 1);
                            if (draft.likeCount) draft.likeCount -= 1;
                            draft.likedByCurrentUser = false;
                        } else {
                            if (!draft.likes) draft.likes = [];
                            draft.likes.push({ userId: userId, templateId: templateId, });
                            if (draft.likeCount !== undefined) draft.likeCount += 1;
                            draft.likedByCurrentUser = true;
                        }
                    })
                );
                try {
                    await queryFulfilled;
                } catch {
                    patchResult.undo(); 
                }
            },

        }),


        addComment: builder.mutation({
            query: (commentData) => ({ 
                url: `/comments`, 
                method: 'POST',
                body: commentData,
            }),
            
            invalidatesTags: (result, error, { templateId }) => [{ type: 'Template', id: templateId }],
            
        }),

        deleteComment: builder.mutation({
            query: (commentId) => ({
                url: `/comments/${commentId}`,
                method: 'DELETE',
            }),
            
            invalidatesTags: (result, error, commentId) => [
                { type: 'Comment', id: commentId },
            ],
        }),
    }),
});

export const {
    useGetTemplatesQuery,
    useGetTemplateByIdQuery,
    useGetLatestPublicTemplatesQuery,
    useGetPopularPublicTemplatesQuery,
    useCreateTemplateMutation,
    useUpdateTemplateMutation,
    useDeleteTemplateMutation,
    useAddQuestionMutation,
    useRemoveQuestionMutation,
    useUpdateQuestionMutation,
    useReorderQuestionsMutation,
    useSetTagsMutation,
    useSetAccessMutation,
    useToggleLikeMutation,
    useAddCommentMutation,
    useDeleteCommentMutation,
} = templatesApi;