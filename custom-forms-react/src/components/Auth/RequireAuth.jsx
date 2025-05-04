import { useLocation, Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { Spinner } from 'react-bootstrap';

const RequireAuth = ({ roles }) => {
    const { isAuthenticated, user } = useAuth(); 
    const location = useLocation();

    if (!isAuthenticated) {
        console.log('[RequireAuth] Not authenticated, redirecting to login.');
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    if (roles && roles.length > 0 && !user) {
        console.log('[RequireAuth] Authenticated, but user data is loading (roles required).');
        return (
            <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
                <Spinner animation="border" variant="primary" />
            </div>
        );
    }

    if (user && roles && roles.length > 0) {
        const userRoles = user.roles || []; 
        const hasRequiredRole = roles.some(role => userRoles.includes(role));

        if (!hasRequiredRole) {
            console.warn(`[RequireAuth] User ${user.userName} does not have required roles: ${roles.join(', ')} for ${location.pathname}`);
            return <Navigate to="/" state={{ from: location }} replace />; 
        }
    }

    console.log('[RequireAuth] Access granted.');
    return <Outlet />;
};

export default RequireAuth;