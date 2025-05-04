import { apiSlice } from './apiSlice';

export const usersApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({

        getCurrentUser: builder.query({
            query: () => '/users/me',
            providesTags: ['UserSelf'], 
        }),

        getUsers: builder.query({
            query: (params) => ({
                url: '/users',
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
                        ...result.items.map(({ id }) => ({ type: 'User', id })),
                        { type: 'User', id: 'LIST' },
                    ]
                    : [{ type: 'User', id: 'LIST' }],
        }),

        blockUser: builder.mutation({
            query: (userId) => ({
                url: `/users/${userId}/block`,
                method: 'PUT',
            }),
            invalidatesTags: (result, error, userId) => [{ type: 'User', id: userId }, { type: 'User', id: 'LIST' }],
        }),

        unblockUser: builder.mutation({
            query: (userId) => ({
                url: `/users/${userId}/unblock`,
                method: 'PUT',
            }),
            invalidatesTags: (result, error, userId) => [{ type: 'User', id: userId }, { type: 'User', id: 'LIST' }],
        }),

        deleteUser: builder.mutation({
            query: (userId) => ({
                url: `/users/${userId}`,
                method: 'DELETE',
            }),
            invalidatesTags: (result, error, userId) => [{ type: 'User', id: userId }, { type: 'User', id: 'LIST' }],
        }),

        setUserRole: builder.mutation({
            query: (body) => ({
                url: `/users/set-role`,
                method: 'POST',
                body: body,
            }),
            invalidatesTags: (result, error, { userId }) => [{ type: 'User', id: userId }, { type: 'User', id: 'LIST' }],
        }),
    }),
});

export const {
    useGetCurrentUserQuery,
    useGetUsersQuery,
    useLazyGetUsersQuery,
    useBlockUserMutation,
    useUnblockUserMutation,
    useDeleteUserMutation,
    useSetUserRoleMutation,
} = usersApi;