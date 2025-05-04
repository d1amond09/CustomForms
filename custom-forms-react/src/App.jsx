import React, { useState, useEffect } from 'react'; 
import { Routes, Route } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import HomePage from './features/home/pages/HomePage';
import LoginPage from './features/auth/components/LoginPage';
import RegisterPage from './features/auth/components/RegisterPage';
import TemplateDetailPage from './features/templates/pages/TemplateDetailPage';
import CreateTemplatePage from './features/templates/pages/CreateTemplatePage';
import FillFormPage from './features/forms/pages/FillFormPage';
import ProfilePage from './features/profile/pages/ProfilePage';
import SearchResultsPage from './features/search/pages/SearchResultsPage';
import AdminUsersPage from './features/admin/pages/AdminUsersPage';
import RequireAuth from './components/Auth/RequireAuth';
import NotFoundPage from './components/Common/NotFoundPage';
import { Spinner } from 'react-bootstrap';
import { useAuth } from './hooks/useAuth';
import { useAppDispatch } from './app/hooks';
import { useGetCurrentUserQuery } from './app/api/usersApi';
import { setUserDetails, logOut } from './features/auth/authSlice';
import { toast } from 'react-toastify';
import { useTranslation } from 'react-i18next';
function App() {

    const { t } = useTranslation();
    const [theme, setTheme] = useState(() => localStorage.getItem('bsTheme') || 'light');

    const { isAuthenticated, user } = useAuth();
    const dispatch = useAppDispatch();
    const { data: currentUserData, isLoading: isLoadingUser, isError: isUserError, error: userError } = useGetCurrentUserQuery(undefined, {
        skip: !isAuthenticated || !!user,
    });

    useEffect(() => {
        if (currentUserData) {
            dispatch(setUserDetails(currentUserData));
        }
        if (isUserError && userError) {
            console.error("Failed to fetch user data:", userError);
            if (userError.status === 401 || userError.status === 403) {
                toast.error(t('errors.sessionExpired'));
                dispatch(logOut());
            } else {
                toast.error(t('errors.loadUserFailed'));
            }
        }
    }, [currentUserData, isUserError, userError, dispatch]);


    useEffect(() => {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('bsTheme', theme);
    }, [theme]);

    const toggleTheme = () => {
        setTheme(prev => prev === 'light' ? 'dark' : 'light');
    };


    return (
        <Layout currentTheme={theme} onToggleTheme={toggleTheme}>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/search" element={<SearchResultsPage />} />
                <Route path="/templates/:id" element={<TemplateDetailPage />} />

                <Route element={<RequireAuth />}>
                    <Route path="/templates/new" element={<CreateTemplatePage />} />
                    <Route path="/forms/fill/:templateId" element={<FillFormPage />} />
                    <Route path="/profile" element={<ProfilePage />} />
                </Route>

                <Route element={<RequireAuth roles={['Admin']} />}>
                    <Route path="/admin/users" element={<AdminUsersPage />} />
                </Route>

                <Route path="*" element={<NotFoundPage />} />
            </Routes>
        </Layout>
    );
}

export default App;