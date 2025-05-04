import { apiSlice } from './apiSlice';

export const formsApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        getForms: builder.query({
            query: (params) => ({ 
                url: '/forms',
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
                        ...result.items.map(({ id }) => ({ type: 'Form', id })),
                        { type: 'Form', id: 'LIST' },
                    ]
                    : [{ type: 'Form', id: 'LIST' }],
        }),

        getFormById: builder.query({
            query: (id) => `/forms/${id}`,
            providesTags: (result, error, id) => [{ type: 'Form', id }],
        }),

        submitForm: builder.mutation({
            query: (formData) => ({ 
                url: '/forms',
                method: 'POST',
                body: formData,
            }),
            invalidatesTags: (result, error, arg) => [
                { type: 'Form', id: 'LIST' },
                { type: 'Template', id: arg.templateId } 
            ],
        }),

        deleteForm: builder.mutation({
            query: (id) => ({
                url: `/forms/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: (result, error, id) => [{ type: 'Form', id }, { type: 'Form', id: 'LIST' }],
        }),

    }),
});

export const {
    useGetFormsQuery,
    useGetFormByIdQuery,
    useSubmitFormMutation,
    useDeleteFormMutation,
} = formsApi;