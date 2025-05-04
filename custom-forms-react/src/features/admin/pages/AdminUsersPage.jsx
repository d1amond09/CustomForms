import React, { useState } from 'react';
import { Container, Alert, Spinner } from 'react-bootstrap';
import UserManagementTable from '../components/UserManagementTable';
import PaginationComponent from '../../../components/Common/PaginationComponent'; 
import { useGetUsersQuery } from '../../../app/api/usersApi'; 
import { useTranslation } from 'react-i18next';

const AdminUsersPage = () => {
    const { t } = useTranslation();
    const [page, setPage] = useState(1);
    const pageSize = 15; 


    const queryParams = {
        pageNumber: page,
        pageSize: pageSize,
        orderBy: 'UserName', 
    };

    const { data, error, isLoading, isFetching } = useGetUsersQuery(queryParams);

    const users = data?.items;
    const metaData = data?.metaData;

    const handlePageChange = (newPage) => {
        setPage(newPage);
    };

    const renderContent = () => {
        if (isLoading || isFetching) return <div className="text-center p-5"><Spinner animation="border" /></div>;
        if (error) return <Alert variant="danger">{t('admin.users.errorLoading', 'Error loading users.')}{JSON.stringify(error)}</Alert>;
        if (!users || users.length === 0) {
            return <p className="text-muted text-center">{t('admin.users.noUsers', 'No users found.')}</p>;
        }

        return (
            <> 
                <UserManagementTable users={users} />

                {metaData && metaData.TotalPages > 1 && (
                    <PaginationComponent
                        currentPage={metaData.CurrentPage}
                        totalPages={metaData.TotalPages}
                        onPageChange={handlePageChange} 
                    />
                )}
            </>
        );
    }

    return (
        <Container fluid="xl" className="my-4">
            <h2 className="mb-4">{t('admin.users.title', 'User Management')}</h2>
            {renderContent()}
        </Container>
    );
};

export default AdminUsersPage;