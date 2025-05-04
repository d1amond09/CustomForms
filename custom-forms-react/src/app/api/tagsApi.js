import { apiSlice } from './apiSlice';

export const tagsApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        getTags: builder.query({
            query: (params) => ({
                url: '/tags',
                params: params,
            }),
            providesTags: (result, error, arg) =>
                result?.items
                    ? [
                        ...result.items.map(({ id }) => ({ type: 'Tag', id })),
                        { type: 'Tag', id: 'LIST' },
                    ]
                    : [{ type: 'Tag', id: 'LIST' }],
        }),

        getPopularTags: builder.query({
            query: (count = 20) => `/tags/popular?count=${count}`,
            providesTags: (result, error, arg) => [{ type: 'Tag', id: 'POPULAR_LIST' }],
        }),

        searchTagsByName: builder.query({
            query: ({ namePrefix, maxResults = 10 }) => ({
                url: '/tags/search-by-name',
                params: { namePrefix, maxResults },
            }),
        }),
    }),
});

export const {
    useGetTagsQuery,
    useGetPopularTagsQuery,
    useLazySearchTagsByNameQuery,
} = tagsApi;