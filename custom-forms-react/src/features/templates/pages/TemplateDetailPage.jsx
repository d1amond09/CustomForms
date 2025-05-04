import React from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { Container, Row, Col, Tabs, Tab, Spinner, Alert, Card, Badge, Button, Stack, Image } from 'react-bootstrap';
import { toast } from 'react-toastify';
import {
    useGetTemplateByIdQuery,
    useToggleLikeMutation,
    useAddCommentMutation,
    useDeleteCommentMutation,
    useDeleteTemplateMutation
} from '../../../app/api/templatesApi';
import { useAuth } from '../../../hooks/useAuth';
import { useTranslation } from 'react-i18next';
import { BsStar, BsStarFill, BsTrash } from 'react-icons/bs';
import QuestionListEditor from '../components/QuestionListEditor';
import CommentSection from '../components/CommentSection';
import TemplateSettings from '../components/TemplateSettings';
import ResultsViewer from '../components/ResultsViewer';

const TemplateDetailPage = () => {
    const { id: templateId } = useParams();
    const { t } = useTranslation();
    const { isAuthenticated, user: currentUser } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    const { data: template, error, isLoading, isError, refetch } = useGetTemplateByIdQuery(templateId, {
        refetchOnMountOrArgChange: true,
    });
    const [toggleLike, { isLoading: isLiking }] = useToggleLikeMutation();
    const [addComment, { isLoading: isAddingComment }] = useAddCommentMutation();
    const [deleteComment] = useDeleteCommentMutation();
    const [deleteTemplate, { isLoading: isDeletingTemplate }] = useDeleteTemplateMutation();

    const handleLikeToggle = async () => {
        if (!isAuthenticated) {
            toast.warning(t('common.loginRequiredAction', 'Please log in to perform this action.'));
            return;
        }
        try {
            await toggleLike(templateId).unwrap();
        } catch (err) {
            toast.error(t('templates.detail.likeError', 'Failed to update like.'));
        }
    };

    const handleAddComment = async (text) => {
        if (!text.trim() || !isAuthenticated) return;
        try {
            await addComment({ templateId, text }).unwrap();
            toast.success(t('templates.detail.commentAdded', 'Comment added!'));
        } catch (err) {
            toast.error(err?.data?.message || t('templates.detail.commentAddError', 'Failed to add comment.'));
        }
    };

    const handleDeleteComment = async (commentId) => {
        if (!window.confirm(t('templates.detail.deleteCommentConfirm', 'Are you sure you want to delete this comment?'))) return;
        try {
            await deleteComment(commentId).unwrap();
            refetch();
            toast.success(t('templates.detail.commentDeleted', 'Comment deleted!'));
        } catch (err) {
            toast.error(err?.data?.message || t('templates.detail.commentDeleteError', 'Failed to delete comment.'));
        }
    };

    const handleDeleteTemplate = async () => {
        if (window.confirm(t('templates.detail.deleteConfirm', 'Are you sure you want to delete this template? This will delete all its questions but not the submitted forms.'))) {
            try {
                await deleteTemplate(templateId).unwrap();
                toast.success(t('templates.detail.deleteSuccess', 'Template deleted successfully.'));
                navigate('/profile');
            } catch (err) {
                toast.error(err?.data?.message || t('templates.detail.deleteError', 'Failed to delete template. Check if there are submitted forms.'));
            }
        }
    };

    const handleSettingsUpdated = () => {
        refetch();
    };

    if (isLoading) {
        return <div className="text-center p-5"><Spinner animation="border" variant="primary" /></div>;
    }

    if (isError || !template) {
        const errorMsg = error?.status === 404 ? t('templates.detail.notFound', 'Template not found.')
            : error?.status === 403 ? t('templates.detail.accessDenied', 'You do not have permission to view this template.')
                : t('templates.detail.errorLoading', 'Error loading template details.');
        return <Container className="mt-4"><Alert variant="danger">{errorMsg}</Alert></Container>;
    }

    const {
        title = 'N/A', description = '', authorName = 'Unknown', topicName = 'N/A',
        createdDate = new Date().toISOString(), imageUrl = null, questions = [], tags = [],
        comments = [], likeCount = 0, likedByCurrentUser = false, isPublic = true,
        allowedUsers = [], canCurrentUserManage = false, authorId = null
    } = template;

    const isAdmin = currentUser?.roles?.includes('Admin');

    const canFillThisForm = isAuthenticated && currentUser && !isAdmin &&
        authorId !== currentUser.id &&
        (isPublic || allowedUsers.some(u => u.id === currentUser.id));

    const dateFormatter = new Intl.DateTimeFormat(t('localeCode', 'en-US'));

    return (
        <Container fluid="xl" className="my-4">
            <Row>
                <Col xs={12} lg={8} className="mb-4 mb-lg-0">
                    <Card className="shadow-sm mb-4">
                        <Card.Body>
                            {canCurrentUserManage && (
                                <Stack direction="horizontal" gap={2} className="mb-3 justify-content-end border-bottom pb-2">
                                    <span className='me-auto text-muted small'>{t('templates.detail.managementActions', 'Management Actions:')}</span>
                                    <Button
                                        variant="outline-danger" size="sm"
                                        onClick={handleDeleteTemplate} disabled={isDeletingTemplate}
                                        title={t('templates.detail.deleteTemplateTitle', 'Delete this template')}
                                    >
                                        {isDeletingTemplate ? <Spinner size="sm" animation="border" /> : <BsTrash className='me-1' />} {t('common.delete', 'Delete')} Template
                                    </Button>
                                </Stack>
                            )}
                            <Card.Title as="h1" className='mb-1'>{title}</Card.Title>
                            <div className="d-flex justify-content-between align-items-center mb-2 text-muted flex-wrap gap-2 small">
                                <span>{t('templates.detail.byAuthor', 'By: {{authorName}}', { authorName })}</span>
                                <span><Badge bg="secondary">{topicName}</Badge></span>
                                <span>
                                    <Badge bg={isPublic ? 'success-subtle' : 'warning-subtle'} text={isPublic ? 'success-emphasis' : 'warning-emphasis'}>
                                        {isPublic ? t('templates.detail.statusPublic', 'Public') : t('templates.detail.statusRestricted', 'Restricted')}
                                    </Badge>
                                </span>
                                <span>{t('templates.detail.createdDate', 'Created: {{date}}', { date: dateFormatter.format(new Date(createdDate)) })}</span>
                            </div>
                            {imageUrl && <Image src={imageUrl} alt={title} fluid rounded className="my-3" style={{ maxHeight: '350px', objectFit: 'cover', width: '100%' }} />}
                            <div className="mb-3 description-content">
                                {description ? (
                                    <p>{description}</p>
                                ) : (
                                    <p className="text-muted fst-italic">{t('templates.detail.noDescription', 'No description provided.')}</p>
                                )}
                            </div>
                            {tags.length > 0 && (
                                <div className="mb-3">
                                    {tags.map(tag => <Badge pill bg="info" text="dark" key={tag.id} className="me-1 mb-1">{tag.name}</Badge>)}
                                </div>
                            )}
                            {!isPublic && (
                                <div className="mb-3">
                                    <small className="text-muted d-block mb-1">{t('templates.detail.allowedUsers', 'Allowed Users:')}</small>
                                    {allowedUsers.length > 0 ? (
                                        allowedUsers.map(u => <Badge pill bg="primary" key={u.id} className="me-1">{u.userName || u.email}</Badge>)
                                    ) : (
                                        <Badge pill bg="light" text="dark">{t('templates.detail.noAllowedUsers', 'No specific users')}</Badge>
                                    )}
                                </div>
                            )}
                            <div className="d-flex justify-content-between align-items-center border-top pt-3">
                                {canFillThisForm ? (
                                    <Button variant="primary" onClick={() => navigate(`/forms/fill/${templateId}`)}>
                                        {t('templates.detail.fillForm', 'Fill Out Form')}
                                    </Button>
                                ) : (
                                    !isAuthenticated ? (
                                        <Button variant="outline-primary" onClick={() => navigate('/login', { state: { from: location.pathname } })}>
                                            {t('templates.detail.loginToFill', 'Login to Fill')}
                                        </Button>
                                    ) : <div />
                                )}
                                <Button
                                    variant="link" onClick={handleLikeToggle} disabled={!isAuthenticated || isLiking}
                                    className="p-0 text-decoration-none d-flex align-items-center"
                                    title={isAuthenticated ? (likedByCurrentUser ? t('templates.detail.unlike', 'Unlike') : t('templates.detail.like', 'Like')) : t('templates.detail.loginToLike', 'Login to like')}
                                >
                                    {isLiking ? <Spinner size="sm" /> : (likedByCurrentUser ? <BsStarFill size={20} color="orange" className='me-1' /> : <BsStar size={20} className='me-1' />)}
                                    <span className='ms-1'>{likeCount}</span>
                                </Button>
                            </div>
                        </Card.Body>
                    </Card>
                    <CommentSection
                        comments={comments} isLoading={isLoading} isAddingComment={isAddingComment}
                        onAddComment={handleAddComment} onDeleteComment={handleDeleteComment}
                        currentUserId={currentUser?.id} isAdmin={isAdmin} isAuthenticated={isAuthenticated}
                    />
                </Col>
                <Col xs={12} lg={4}>
                    <Tabs defaultActiveKey="questions" id="template-details-tabs" className="mb-3 template-tabs sticky-lg-top bg-body" mountOnEnter>
                        <Tab eventKey="questions" title={t('templates.detail.tabQuestions', 'Questions ({{count}})', { count: questions.length })} className="py-3">
                            <QuestionListEditor
                                templateId={templateId} initialQuestions={questions}
                                canManage={canCurrentUserManage} onQuestionsUpdated={refetch}
                            />
                        </Tab>
                        {canCurrentUserManage && (
                            <Tab eventKey="results" title={t('templates.detail.tabResults', 'Results')} className="py-3">
                                <ResultsViewer templateId={templateId} />
                            </Tab>
                        )}
                        {canCurrentUserManage && (
                            <Tab eventKey="settings" title={t('templates.detail.tabSettings', 'Settings')} className="py-3">
                                <TemplateSettings template={template} onSettingsUpdated={handleSettingsUpdated} />
                            </Tab>
                        )}
                    </Tabs>
                </Col>
            </Row>
            <style>{`/* ... styles ... */`}</style>
        </Container>
    );
};

export default TemplateDetailPage;