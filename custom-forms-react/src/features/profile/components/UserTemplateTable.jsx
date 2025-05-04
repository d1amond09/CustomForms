import React, { useState } from 'react';
import { Table, Button, Spinner, Alert, Stack, Badge } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { useGetTemplatesQuery, useDeleteTemplateMutation } from '../../../app/api/templatesApi';
import PaginationComponent from '../../../components/Common/PaginationComponent';
import { BsEye, BsPencil, BsTrash } from 'react-icons/bs';
import { toast } from 'react-toastify';
import { useTranslation } from 'react-i18next';

const UserTemplateTable = ({ userId }) => {
    const { t } = useTranslation();
    const [page, setPage] = useState(1);
    const pageSize = 10;
    const queryParams = {
        pageNumber: page,
        pageSize: pageSize,
        authorId: userId,
        orderBy: 'CreatedDate desc'
    };

    const {
        data,
        error,
        isLoading,
        isFetching,
    } = useGetTemplatesQuery(queryParams);

    const [deleteTemplate, { isLoading: isDeleting }] = useDeleteTemplateMutation();

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this template? Associated forms might prevent deletion if they exist. This action cannot be undone.')) {
            try {
                await deleteTemplate(id).unwrap();
                toast.success('Template deleted successfully.');
            } catch (err) {
                const errorMessage = err?.data?.message?.includes("form(s) have been submitted")
                    ? "Cannot delete: Submitted forms exist for this template."
                    : (err?.data?.message || 'Failed to delete template.');
                toast.error(errorMessage);
            }
        }
    };

    const handlePageChange = (newPage) => {
        if (newPage !== page) {
            setPage(newPage);
        }
    };

    if (isLoading) return <div className="text-center p-3"><Spinner animation="border" /></div>;
    if (error) return <Alert variant="danger">Error loading your templates. {JSON.stringify(error.data?.message || error.status)}</Alert>;

    const templates = data?.items;
    const metaData = data?.metaData;

    if (!templates || templates.length === 0) return <p className="text-muted">You haven't created any templates yet.</p>;

    return (
        <>
            {isFetching && !isLoading && <div className="text-center pb-2"><Spinner animation="border" size="sm" /></div>}

            <Table striped bordered hover responsive size="sm" className="user-templates-table">
                <thead className='table-light'>
                    <tr>
                        <th>{t('templates.detail.title', 'Title')}</th>
                        <th> {t('templates.detail.topic', 'Topic')}</th>
                        <th>{t('templates.detail.status', 'Status')}</th>
                        <th>{t('templates.detail.createdDate', 'Created')}</th>
                        <th>{t('templates.detail.responses', 'Responses')}</th>
                        <th className="text-center">{t('templates.detail.actions', 'Actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {templates.map(template => (
                        <tr key={template.id}>
                            <td> 
                                <LinkContainer to={`/templates/${template.id}`}>
                                    <a href={`/templates/${template.id}`} className="text-primary text-decoration-none">{template.title}</a>
                                </LinkContainer>
                            </td>
                            <td><Badge bg="secondary">{template.topicName || 'N/A'}</Badge></td>
                            <td>
                                <Badge bg={template.isPublic ? 'success-subtle' : 'warning-subtle'} text={template.isPublic ? 'success-emphasis' : 'warning-emphasis'}>
                                    {template.isPublic ? t('templates.detail.statusPublic', 'Public') : t('templates.detail.statusRestricted', 'Restricted')}
                                </Badge>
                            </td>
                            <td>{new Date(template.createdDate).toLocaleDateString()}</td>
                            <td>{template.formCount ?? 0}</td>
                            <td className="text-center">
                                <Stack direction="horizontal" gap={1} className="justify-content-center">
                                    <LinkContainer to={`/templates/${template.id}`}>
                                        <Button variant="outline-primary" size="sm" title="View Details"><BsPencil /></Button>
                                    </LinkContainer>
                                    <Button
                                        variant="outline-danger"
                                        size="sm"
                                        title="Delete Template"
                                        onClick={() => handleDelete(template.id)}
                                        disabled={isDeleting}
                                    >
                                        <BsTrash />
                                    </Button>
                                </Stack>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>

            {metaData && metaData.TotalPages > 1 && (
                <PaginationComponent
                    currentPage={metaData.CurrentPage}
                    totalPages={metaData.TotalPages}
                    onPageChange={handlePageChange}
                />
            )}
        </>
    );
};

export default UserTemplateTable;