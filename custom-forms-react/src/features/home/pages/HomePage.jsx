import React from 'react';
import { Row, Col, Tabs, Tab, Spinner, Alert } from 'react-bootstrap';
import { useGetLatestPublicTemplatesQuery, useGetPopularPublicTemplatesQuery } from '../../../app/api/templatesApi';
import TemplateCard from '../../templates/components/TemplateCard'; 
import TagCloud from '../components/TagCloud'; 
import { useTranslation } from 'react-i18next';

const HomePage = () => {
    const { t } = useTranslation();
    const { data: latestData, isLoading: isLoadingLatest, isError: isErrorLatest } = useGetLatestPublicTemplatesQuery(6);
    const { data: popularData, isLoading: isLoadingPopular, isError: isErrorPopular } = useGetPopularPublicTemplatesQuery(6);

    const renderTemplateList = (data, isLoading, isError) => {
        if (isLoading) return <div className="text-center p-5"><Spinner animation="border" variant="primary" /></div>;
        if (isError) return <Alert variant="danger">{t('home.errorLoadingTemplates', 'Error loading templates')}</Alert>;

        const templates = data?.items || data;
        if (!templates || templates.length === 0) return <p className="text-muted text-center py-3">{t('home.noTemplatesFound', 'No templates found.')}</p>;

        return (
            <Row xs={1} md={2} lg={3} className="g-4">
                {templates.map((template) => (
                    <Col key={template.id}>
                        <TemplateCard template={template} />
                    </Col>
                ))}
            </Row>
        );
    };

    return (
        <>
            <div className="text-center my-4 my-md-5">
                <h1>{t('home.welcomeTitle', 'Welcome to Custom Forms!')}</h1>
                <p className="lead text-body-secondary">{t('home.welcomeSubtitle', 'Create and share your forms easily.')}</p>
            </div>

            <Tabs defaultActiveKey="latest" id="home-template-tabs" className="mb-3 mb-md-4 justify-content-center nav-pills-custom"> 
                <Tab eventKey="latest" title={t('home.latestTemplates', 'Latest Templates')}>
                    <div className='mt-4'>
                        {renderTemplateList(latestData?.items || latestData, isLoadingLatest, isErrorLatest)}
                    </div>
                </Tab>
                <Tab eventKey="popular" title={t('home.popularTemplates', 'Popular Templates')}>
                    <div className='mt-4'>
                        {renderTemplateList(popularData?.items || popularData, isLoadingPopular, isErrorPopular)}
                    </div>
                </Tab>
            </Tabs>

            <div className='mt-5 pt-4 border-top'>
                <h2 className="mb-3">{t('home.tags', 'Tags')}</h2>
                <TagCloud />
            </div>
            <style>{`
            .nav-pills-custom .nav-link {
                color: var(--bs-secondary-color);
                background: none;
                border: 0;
                border-bottom: 2px solid transparent;
                border-radius: 0;
                padding-left: 1rem;
                padding-right: 1rem;
             }
            .nav-pills-custom .nav-link:hover {
                color: var(--bs-primary);
            }
            .nav-pills-custom .nav-link.active {
                color: var(--bs-primary);
                background-color: transparent;
                border-bottom-color: var(--bs-primary);
            }
          `}
           </style>
        </>
    );
};

export default HomePage;