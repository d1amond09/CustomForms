import { useMemo } from 'react';
import { useAppSelector } from '../app/hooks';
import { selectIsAuthenticated, selectCurrentUser } from '../features/auth/authSlice';

export const useAuth = () => {
    const isAuthenticated = useAppSelector(selectIsAuthenticated);
    const user = useAppSelector(selectCurrentUser); 
    const isAdmin = useMemo(() => user?.roles?.includes('Admin'), [user]);
    return useMemo(() => ({ isAuthenticated, user, isAdmin }), [isAuthenticated, user, isAdmin]);
};