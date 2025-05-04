import React from 'react';
import { useParams, Link as RouterLink } from 'react-router-dom';
import { Container, Row, Col, Card, Spinner, Alert, ListGroup, Badge } from 'react-bootstrap';
import { useGetFormByIdQuery } from '../../../app/api/formsApi';
import { useAuth } from '../../../hooks/useAuth';
import { useTranslation } from 'react-i18next';
import { BsCalendarEvent, BsPerson, BsLayoutTextWindow } from 'react-icons/bs';

const ViewSubmittedFormPage = () => {
    const { id: formId } = useParams();
    const { t } = useTranslation();
    const { user: currentUser, isAuthenticated } = useAuth();

    const { data: form, error, isLoading, isError, isFetching } = useGetFormByIdQuery(formId);

    if (isLoading || isFetching) {
        return (
            <Container className="text-center mt-5">
                <Spinner animation="border" variant="primary" />
                <p>{t('viewForm.loading', 'Loading form details...')}</p>
            </Container>
        );
    }

    if (isError || !form) {
        let errorMessage = t('viewForm.errors.loadFailed', 'Could not load the submitted form details.');
        if (error?.status === 404) {
            errorMessage = t('viewForm.errors.notFound', 'The form was not found.');
        } else if (error?.status === 403) {
            errorMessage = t('viewForm.errors.accessDenied', 'You do not have permission to view this form.');
        } else if (error) {
            errorMessage += ` (${error.status || 'Network error'})`;
        }
        return (
            <Container className="mt-5">
                <Alert variant="danger">{errorMessage}</Alert>
            </Container>
        );
    }

    const {
        templateTitle,
        templateId,
        userName: submitterName,
        userId: submitterId,
        filledDate,
        answers = []
    } = form;

    const formatAnswerValue = (value, type) => {
        if (type === 'Checkbox') {
            return value === 'true' ? 'Yes' : 'No';
        }
        if (type === 'Text') {
            return value || <span className="text-muted fst-italic">(empty)</span>;
        }
        return value || <span className="text-muted fst-italic">(empty)</span>;
    };

    return (
        <Container fluid="xl" className="my-4">
            <Row className="justify-content-center">
                <Col lg={9} md={10}>
                    <Card className="shadow-sm">
                        <Card.Header>
                            <Card.Title as="h2" className="mb-0">
                                {t('viewForm.title', 'Submitted Form Details')}
                            </Card.Title>
                        </Card.Header>
                        <Card.Body>
                            <ListGroup variant="flush" className="mb-4">
                                <ListGroup.Item className="d-flex justify-content-between align-items-center px-0">
                                    <span><BsLayoutTextWindow className="me-2" /> {t('viewForm.templateLabel', 'Template:')}</span>
                                    <strong>
                                        <RouterLink to={`/templates/${templateId}`} className="text-decoration-none">
                                            {templateTitle || t('viewForm.notAvailable', 'N/A')}
                                        </RouterLink>
                                    </strong>
                                </ListGroup.Item>
                                <ListGroup.Item className="d-flex justify-content-between align-items-center px-0">
                                    <span><BsPerson className="me-2" />{t('viewForm.submittedByLabel', 'Submitted By:')}</span>
                                    <strong>{submitterName || t('viewForm.unknownUser', 'Unknown User')}</strong>
                                </ListGroup.Item>
                                <ListGroup.Item className="d-flex justify-content-between align-items-center px-0">
                                    <span><BsCalendarEvent className="me-2" /> {t('viewForm.submittedOnLabel', 'Submitted On:')}</span>
                                    <strong>{new Date(filledDate).toLocaleString()}</strong>
                                </ListGroup.Item>
                            </ListGroup>

                            <hr />

                            <h4 className="mb-3">{t('viewForm.answersTitle', 'Answers')}</h4>
                            {answers.length > 0 ? (
                                <ListGroup variant="flush">
                                    {answers.map((answer, index) => (
                                        <ListGroup.Item key={answer.id || `answer-${index}`} className="px-0 py-3">
                                            <div className="d-flex w-100 justify-content-between">
                                                <h6 className="mb-1 fw-bold">{answer.questionTitle || t('viewForm.questionIdPlaceholder', 'Question ID: {{id}}', { id: answer.questionId })}</h6>
                                                <small className="text-muted">{answer.questionType}</small>
                                            </div>
                                            <p className="mb-1" style={{ whiteSpace: 'pre-wrap' }}>
                                                {formatAnswerValue(answer.value, answer.questionType)}
                                            </p>
                                        </ListGroup.Item>
                                    ))}
                                </ListGroup>
                            ) : (
                                    <p className="text-muted">{t('viewForm.noAnswers', 'No answers were recorded for this submission.')}</p>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default ViewSubmittedFormPage;