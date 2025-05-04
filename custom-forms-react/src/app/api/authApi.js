import { apiSlice } from './apiSlice';
import { usersApi } from './usersApi';
import { setCredentials, logOut } from '../../features/auth/authSlice';

export const authApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        login: builder.mutation({
            query: (credentials) => ({
                url: '/auth/login',
                method: 'POST',
                body: credentials, 
            }),

            async onQueryStarted(arg, { dispatch, queryFulfilled }) {
                try {
                    const { data: tokenData } = await queryFulfilled;
                    if (!tokenData?.accessToken || !tokenData?.refreshToken) {
                        console.error("Login did not return valid tokens:", tokenData);
                        throw new Error('Invalid token response from server.');
                    }

                    dispatch(setCredentials({ accessToken: tokenData.accessToken, refreshToken: tokenData.refreshToken, user: null })); 
                    const userResult = await dispatch(usersApi.endpoints.getCurrentUser.initiate(undefined, { forceRefetch: true }));
                    if (userResult.isError || !userResult.data) {
                        console.error("Failed to fetch user data after login:", userResult.error);
                        dispatch(logOut());
                        throw new Error('Failed to fetch user data after login');
                    }
                    const userData = userResult.data;
                    dispatch(setCredentials({
                        accessToken: tokenData.accessToken,
                        refreshToken: tokenData.refreshToken,
                        user: userData
                    }));
                } catch (error) {
                    console.error('Login query failed:', error);
                    dispatch(logOut());
                }
            },
        }),


        register: builder.mutation({
            query: (userData) => ({
                url: '/auth/register',
                method: 'POST',
                body: userData, 
            }),
           
        }),

        refreshToken: builder.mutation({
            query: (tokens) => ({ 
                url: '/token/refresh',
                method: 'POST',
                body: tokens,
            }),

            async onQueryStarted(arg, { dispatch, queryFulfilled, getState }) {
                try {
                    const { data: newTokenData } = await queryFulfilled;
                    if (!newTokenData?.accessToken || !newTokenData?.refreshToken) {
                        console.error("Refresh token response invalid:", newTokenData);
                        throw new Error('Invalid token refresh response.');
                    }
                    const currentUser = getState().auth.user;
                    if (currentUser) {
                        dispatch(setCredentials({
                            accessToken: newTokenData.accessToken,
                            refreshToken: newTokenData.refreshToken,
                            user: currentUser
                        }));
                    } else {
                        console.warn("User data not found during token refresh, logging out.");
                        dispatch(logOut());
                    }
                } catch (error) {
                    console.error('Token refresh query failed:', error);
                    dispatch(logOut());
                }
            }
        }),
    }),
    overrideExisting: false,
});

export const {
    useLoginMutation,
    useRegisterMutation,
    useRefreshTokenMutation,
} = authApi;    