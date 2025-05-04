import { apiSlice } from './apiSlice';

export const likesApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        toggleLike: builder.mutation({
            query: (templateId) => ({
                url: `/likes/toggle/${templateId}`,
                method: 'POST',
            }),
            invalidatesTags: (result, error, templateId) => [{ type: 'Template', id: templateId }],
        }),
    }),
});

export const {
    useToggleLikeMutation,
} = likesApi;