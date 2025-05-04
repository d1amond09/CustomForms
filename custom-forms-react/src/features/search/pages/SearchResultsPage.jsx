import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Container, Row, Col, Spinner, Alert } from 'react-bootstrap';
import { useGetTemplatesQuery } from '../../../app/api/templatesApi';
import TemplateCard from '../../templates/components/TemplateCard';
import PaginationComponent from '../../../components/Common/PaginationComponent';
import { useTranslation } from 'react-i18next';

const SearchResultsPage = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();
    const query = searchParams.get('query') || '';
    const tag = searchParams.get('tag');
    const [page, setPage] = useState(1);
    const pageSize = 12;

    const queryParams = {
        pageNumber: page,
        pageSize: pageSize,
        searchTerm: query || tag || '',
        orderBy: 'CreatedDate desc',
    };

    const {
        data,
        error,
        isLoading,
        isFetching,
    } = useGetTemplatesQuery(queryParams, {
        skip: !query && !tag,
    });

    useEffect(() => {
        setPage(1);
    }, [query, tag]);

    const handlePageChange = (newPage) => {
        if (newPage !== page) {
            setPage(newPage);
        }
    };

    const renderContent = () => {
        if (isLoading || isFetching) {
            return <div className="text-center p-5"><Spinner animation="border" variant="primary" /></div>;
        }

        if (error) {
            return <Alert variant="danger">{t('search.error', 'Error loading search results.')} {JSON.stringify(error.data?.message || error.status)}</Alert>;
        }

        const templates = data?.items;
        const metaData = data?.metaData;

        if (!templates || templates.length === 0) {
            return <p className="text-muted text-center py-4">{t('search.noResults', 'No templates found matching your criteria.')}</p>;
        }

        return (
            <>
                <Row xs={1} sm={2} md={3} lg={4} className="g-4">
                    {templates.map(template => (
                        <Col key={template.id}>
                            <TemplateCard template={template} />
                        </Col>
                    ))}
                </Row>

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
            <h2 className="mb-4">
                {t('search.title', 'Search Results')} {query && ` ${t('search.titleForQuery', 'Search Results')} "${query}"`} {tag && `${t('search.titleForTag', 'Search Results')} "${tag}"`}
            </h2>
            {renderContent()}
        </Container>
    );
};

export default SearchResultsPage;