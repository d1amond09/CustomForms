import { apiSlice } from './apiSlice';


export const topicsApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        getTopics: builder.query({
            query: (params = { pageNumber: 1, pageSize: 100, orderBy: 'Name' }) => ({
                url: '/topics',
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
                        ...result.items.map(({ id }) => ({ type: 'Topic', id })),
                        { type: 'Topic', id: 'LIST' },
                    ]
                    : [{ type: 'Topic', id: 'LIST' }],
        }),
    }),
});

export const {
    useGetTopicsQuery,
} = topicsApi;