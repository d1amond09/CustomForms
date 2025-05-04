import React, { useState } from 'react';
import { Table, Button, Spinner, Alert, Stack } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { useGetFormsQuery, useDeleteFormMutation } from '../../../app/api/formsApi';
import PaginationComponent from '../../../components/Common/PaginationComponent';
import { BsEye, BsTrash } from 'react-icons/bs';
import { toast } from 'react-toastify';
import { useTranslation } from 'react-i18next';

const UserFormTable = ({ userId }) => {
    const { t } = useTranslation();
    const [page, setPage] = useState(1);
    const pageSize = 10;

    const queryParams = {
        pageNumber: page,
        pageSize: pageSize,
        userId: userId,
        orderBy: 'FilledDate desc'
    };

    const { data, error, isLoading, isFetching } = useGetFormsQuery(queryParams);
    const [deleteForm, { isLoading: isDeleting }] = useDeleteFormMutation();

    const handleDelete = async (formId) => {
        if (window.confirm('Are you sure you want to delete this submitted form? This action cannot be undone.')) {
            try {
                await deleteForm(formId).unwrap();
                toast.success('Submitted form deleted.');
            } catch (err) {
                toast.error(err?.data?.message || 'Failed to delete the form submission.');
                console.error("Delete form error:", err);
            }
        }
    };

    const handlePageChange = (newPage) => {
        if (newPage !== page) {
            setPage(newPage);
        }
    };

    if (isLoading || isFetching) {
        return <div className="text-center p-3"><Spinner animation="border" /></div>;
    }

    if (error) {
        console.error("Error loading user forms:", error);
        return <Alert variant="danger">Error loading your submitted forms.</Alert>;
    }

    const forms = data?.items;
    const metaData = data?.metaData;

    if (!forms || forms.length === 0) {
        return <p className="text-muted">You haven't submitted any forms yet.</p>;
    }

    const dateFormatter = new Intl.DateTimeFormat(t('localeCode', 'en-US'));
    return (
        <>
            <Table striped bordered hover responsive size="sm">
                <thead>
                    <tr>
                        <th>{t('forms.templateTitle', 'Template Title')}</th>
                        <th>{t('forms.results.colFilledDate', 'Filled Date')}</th>
                        <th>{t('forms.results.colActions', 'Actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {forms.map(form => (
                        <tr key={form.id}>
                            <td>
                                <LinkContainer to={`/templates/${form.templateId}`}>
                                    <a href={`/templates/${form.templateId}`} className="text-decoration-none">{form.templateTitle || 'N/A'}</a>
                                </LinkContainer>
                            </td>
                            <td> {dateFormatter.format(new Date(form.filledDate))}</td>
                    
                            <td>
                                <Stack direction="horizontal" gap={1}>
                                    <LinkContainer to={`/forms/${form.id}`}>
                                        <Button
                                            variant="outline-info"
                                            size="sm"
                                            title={t('forms.results.viewDetails', 'View Details')}>
                                            <BsEye />
                                        </Button>
                                    </LinkContainer>
                                    <Button
                                        variant="outline-danger"
                                        size="sm"
                                        title="Delete Submission"
                                        onClick={() => handleDelete(form.id)}
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

export default UserFormTable;