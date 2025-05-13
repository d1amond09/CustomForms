import { apiSlice } from './apiSlice';

export const supportApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        createSupportTicket: builder.mutation({
            query: (ticketData) => ({ 
                url: '/support/ticket',
                method: 'POST',
                body: ticketData,
            }),
        }),
    }),
});

export const {
    useCreateSupportTicketMutation,
} = supportApi;