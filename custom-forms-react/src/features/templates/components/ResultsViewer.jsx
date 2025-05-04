import React, { useState } from 'react';
import { Table, Button, Spinner, Alert, Stack } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { useGetFormsQuery, useDeleteFormMutation } from '../../../app/api/formsApi';
import PaginationComponent from '../../../components/Common/PaginationComponent';
import { BsEye, BsTrash } from 'react-icons/bs';
import { toast } from 'react-toastify';

const ResultsViewer = ({ templateId }) => {
    const [page, setPage] = useState(1);
    const pageSize = 15;
    const queryParams = { templateId, pageNumber: page, pageSize, orderBy: 'FilledDate desc' };

    const { data, error, isLoading, isFetching } = useGetFormsQuery(queryParams);
    const [deleteForm, { isLoading: isDeleting }] = useDeleteFormMutation();

    const handleDelete = async (formId) => {
        if (window.confirm('Are you sure you want to delete this submission? This action cannot be undone.')) {
            try {
                await deleteForm(formId).unwrap();
                toast.success('Submission deleted successfully.');
            } catch (err) {
                toast.error(err?.data?.message || 'Failed to delete the submission.');
            }
        }
    };

    const handlePageChange = (newPage) => {
        if (newPage !== page) {
            setPage(newPage);
        }
    };

    if (isLoading) {
        return <div className="text-center p-3"><Spinner animation="border" variant="primary" /> <span className='ms-2'>Loading results...</span></div>;
    }

    if (error) {
        return <Alert variant="danger">Error loading results. Please try again later.</Alert>;
    }

    if (!data?.items || data.items.length === 0) {
        return <p className="text-muted text-center py-3">No submissions have been recorded for this template yet.</p>;
    }

    const { items: forms, metaData } = data;

    return (
        <>
            {isFetching && <div className="text-center pb-2"><Spinner animation="border" size="sm" /></div>}

            <Table striped bordered hover responsive size="sm" className="results-table">
                <thead className='table-light'>
                    <tr>
                        <th>#</th>
                        <th>Submitted By</th>
                        <th>Date Submitted</th>
                        <th className="text-center">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {forms.map((form, index) => (
                        <tr key={form.id}>
                            <td>{(metaData.CurrentPage - 1) * metaData.PageSize + index + 1}</td>
                            <td>{form.userName || 'Unknown User'}</td>
                            <td>{new Date(form.filledDate).toLocaleString()}</td>
                            <td className="text-center">
                                <Stack direction="horizontal" gap={1} className="justify-content-center">
                                    <LinkContainer to={`/forms/${form.id}`}>
                                        <Button variant="outline-info" size="sm" title="View Details">
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

export default ResultsViewer;