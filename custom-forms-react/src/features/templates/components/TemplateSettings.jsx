import React, { useState, useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Form, Button, Card, Spinner, Alert, Badge, CloseButton, Stack, ListGroup, InputGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useUpdateTemplateMutation, useSetAccessMutation, useSetTagsMutation } from '../../../app/api/templatesApi';
import { useGetTopicsQuery } from '../../../app/api/topicsApi';
import { useLazyGetUsersQuery } from '../../../app/api/usersApi';
import { BsSearch } from 'react-icons/bs';
import { useTranslation } from 'react-i18next';

const TemplateSettings = ({ template, onSettingsUpdated }) => {
    const { t } = useTranslation();
    const { id: templateId, title, description, topicId, isPublic, tags: currentTags = [], allowedUsers: currentAllowedUsers = [] } = template;

    const { data, isLoading: isLoadingTopics, isError: isErrorTopics } = useGetTopicsQuery({ pageNumber: 1, pageSize: 999 });
    const [updateTemplate, { isLoading: isUpdatingMeta, error: metaError }] = useUpdateTemplateMutation();
    const [setAccess, { isLoading: isUpdatingAccess, error: accessError }] = useSetAccessMutation();
    const [saveTags, { isLoading: isUpdatingTags, error: tagsError }] = useSetTagsMutation();
    const [triggerUserSearch, { data: userSearchResults, isLoading: isSearchingUsers, isFetching: isFetchingUsers }] = useLazyGetUsersQuery();

    const [tags, setTags] = useState([]);
    const [tagInput, setTagInput] = useState('');
    const [allowedUsers, setAllowedUsers] = useState([]);
    const [apiError, setApiError] = useState(null);

    const { register, handleSubmit, control, watch, reset, formState: { errors, isDirty: isMetaDirty } } = useForm({
        defaultValues: {
            title: title || '',
            description: description || '',
            topicId: topicId || '',
            isPublic: isPublic ?? true,
        }
    });
    const watchedIsPublic = watch('isPublic');

    const [tagsChanged, setTagsChanged] = useState(false);
    const [accessChanged, setAccessChanged] = useState(false);

    useEffect(() => {
        reset({
            title: template.title || '',
            description: template.description || '',
            topicId: template.topicId || '',
            isPublic: template.isPublic ?? true,
        });
        setTags(template.tags?.map(t => t.name.toLowerCase()) || []);
        setAllowedUsers(template.allowedUsers || []);
        setTagsChanged(false);
        setAccessChanged(false);
    }, [template, reset]);

    useEffect(() => {
        const initialAllowedIds = (template.allowedUsers || []).map(u => u.id).sort().join(',');
        const currentAllowedIds = allowedUsers.map(u => u.id).sort().join(',');
        setAccessChanged((template.isPublic ?? true) !== watchedIsPublic || initialAllowedIds !== currentAllowedIds);
    }, [watchedIsPublic, allowedUsers, template.isPublic, template.allowedUsers]);

    useEffect(() => {
        const initialTagsString = (template.tags || []).map(t => t.name.toLowerCase()).sort().join(',');
        const currentTagsString = tags.sort().join(',');
        setTagsChanged(initialTagsString !== currentTagsString);
    }, [tags, template.tags]);

    const handleTagInputKeyDown = (e) => {
        if ((e.key === ',' || e.key === 'Enter') && tagInput.trim()) {
            e.preventDefault();
            const newTag = tagInput.trim().toLowerCase();
            if (newTag && !tags.includes(newTag)) {
                setTags([...tags, newTag]);
                setTagsChanged(true);
            }
            setTagInput('');
        }
    };
    const removeTag = (tagToRemove) => {
        setTags(tags.filter(tag => tag !== tagToRemove));
        setTagsChanged(true);
    };

    const handleUserSearchChange = (e) => { };
    const handleAddUser = (user) => {
        if (user && !allowedUsers.some(u => u.id === user.id)) {
            setAllowedUsers([...allowedUsers, user]);
            setUserSearchTerm('');
            setAccessChanged(true);
        }
    };
    const removeUser = (userId) => {
        setAllowedUsers(allowedUsers.filter(u => u.id !== userId));
        setAccessChanged(true);
    };

    const onMetaSubmit = async (data) => {
        setApiError(null);
        const updateData = { title: data.title, description: data.description, topicId: data.topicId };
        try {
            await updateTemplate({ id: templateId, templateData: updateData }).unwrap();
            toast.success(t('templateSettings.updateSuccess.details'));
            reset(data);
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.details');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const onAccessSubmit = async () => {
        setApiError(null);
        const accessData = {
            isPublic: watchedIsPublic,
            allowedUserIds: !watchedIsPublic ? allowedUsers.map(u => u.id) : null
        };
        if (!watchedIsPublic && (!accessData.allowedUserIds || accessData.allowedUserIds.length === 0)) {
            const errorMsg = t('createTemplate.errors.allowedUsersRequired');
            setApiError(errorMsg);
            toast.error(errorMsg);
            return;
        }
        try {
            await setAccess({ templateId, accessData }).unwrap();
            toast.success(t('templateSettings.updateSuccess.access'));
            setAccessChanged(false);
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.access');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const onTagsSubmit = async () => {
        setApiError(null);
        try {
            await saveTags({ templateId, tagNames: tags }).unwrap();
            toast.success(t('templateSettings.updateSuccess.tags'));
            setTagsChanged(false);
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.tags');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const topics = data || [];
    const noTopicsAvailable = !isLoadingTopics && !isErrorTopics && topics.length === 0;

    return (
        <Card>
            <Card.Body>
                <Card.Title as="h5" className='mb-3'>{t('templateSettings.title', 'Template Settings')}</Card.Title>
                {apiError && <Alert variant="danger" className="py-1 px-2 small">{apiError}</Alert>}

                <Form onSubmit={handleSubmit(onMetaSubmit)} className="mb-4 border-bottom pb-4">
                    <h6 className='mb-3'>{t('templateSettings.subtitles.general', 'General Information')}</h6>
                    {(metaError) && <Alert variant="danger" size="sm">{metaError?.data?.message || t('templateSettings.updateError.details')}</Alert>}
                    <Form.Group className="mb-3" controlId="settingsTitle">
                        <Form.Label>{t('createTemplate.labels.title')} <span className="text-danger">*</span></Form.Label>
                        <Form.Control type="text" isInvalid={!!errors.title} {...register('title', { required: t('createTemplate.errors.titleRequired') })} />
                        <Form.Control.Feedback type="invalid">{errors.title?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="settingsDescription">
                        <Form.Label>{t('createTemplate.labels.description')}</Form.Label>
                        <Form.Control as="textarea" rows={3} {...register('description')} />
                        <Form.Text muted>{t('createTemplate.hints.markdown')}</Form.Text>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="settingsTopic">
                        <Form.Label>{t('createTemplate.labels.topic')} <span className="text-danger">*</span></Form.Label>
                        <Form.Select
                            aria-label={t('createTemplate.labels.selectTopic')}
                            isInvalid={!!errors.topicId}
                            {...register('topicId', { required: t('createTemplate.errors.topicRequired') })}
                            disabled={isLoadingTopics || noTopicsAvailable}
                        >
                            <option value="">-- {t('createTemplate.labels.selectTopic')} --</option>
                            {isLoadingTopics && <option>{t('createTemplate.loading', 'Loading...')}</option>}
                            {isErrorTopics && <option disabled>{t('createTemplate.errors.topicLoadFail')}</option>}
                            {!isLoadingTopics && !isErrorTopics && topics.map(topic => (
                                <option key={topic.id} value={topic.id}>{topic.name}</option>
                            ))}
                        </Form.Select>
                        {noTopicsAvailable && (<Alert variant='warning' className='mt-2 py-1 px-2 small'>{t('createTemplate.errors.noTopics')}</Alert>)}
                        <Form.Control.Feedback type="invalid">{errors.topicId?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <Button type="submit" variant="primary" size="sm" disabled={isUpdatingMeta || !isMetaDirty}>
                        {isUpdatingMeta ? <Spinner as="span" animation="border" size="sm" /> : t('templateSettings.buttons.saveDetails')}
                    </Button>
                </Form>

                <Form onSubmit={(e) => { e.preventDefault(); onTagsSubmit(); }} className="mb-4 border-bottom pb-4">
                    <h6 className='mb-3'>{t('createTemplate.labels.tags')}</h6>
                    {(tagsError) && <Alert variant="danger" size="sm">{tagsError?.data?.message || t('templateSettings.updateError.tags')}</Alert>}
                    <Form.Group className="mb-3">
                        <Form.Control
                            type="text"
                            value={tagInput}
                            onChange={(e) => setTagInput(e.target.value)}
                            onKeyDown={handleTagInputKeyDown}
                            placeholder={t('createTemplate.placeholders.tags')}
                        />
                        <div className='mt-2 d-flex flex-wrap gap-1'>
                            {tags.map((tag, index) => (
                                <Badge pill bg="secondary" key={index} className="d-flex align-items-center">
                                    {tag}
                                    <CloseButton className='ms-1' onClick={() => removeTag(tag)} bsPrefix='btn-close-white btn-close' style={{ fontSize: '0.6em' }} aria-label={`Remove ${tag}`} />
                                </Badge>
                            ))}
                        </div>
                        <Form.Text muted>{t('createTemplate.hints.tags')}</Form.Text>
                    </Form.Group>
                    <Button type="submit" variant="primary" size="sm" disabled={isUpdatingTags || !tagsChanged}>
                        {isUpdatingTags ? <Spinner as="span" animation="border" size="sm" /> : t('templateSettings.buttons.saveTags')}
                    </Button>
                </Form>

                <Form onSubmit={(e) => { e.preventDefault(); onAccessSubmit(); }}>
                    <h6 className='mb-3'>{t('templateSettings.subtitles.access')}</h6>
                    {(accessError) && <Alert variant="danger" size="sm">{accessError?.data?.message || t('templateSettings.updateError.access')}</Alert>}
                    <Form.Group className="mb-3">
                        <Controller
                            name="isPublic"
                            control={control}
                            render={({ field }) => (
                                <Form.Check
                                    type="switch"
                                    id="settingsIsPublic"
                                    label={t('createTemplate.labels.isPublic')}
                                    {...field}
                                    checked={field.value}
                                    onChange={(e) => {
                                        field.onChange(e.target.checked);
                                        setAccessChanged(true);
                                    }}
                                />
                            )}
                        />
                    </Form.Group>
                    {!watchedIsPublic && (
                        <Form.Group className="mb-3">
                            <Form.Label>{t('createTemplate.labels.allowedUsers')}</Form.Label>
                            <InputGroup className="mb-2" size="sm">
                                <Form.Control
                                    placeholder={t('createTemplate.placeholders.userSearch')}
                                    value={userSearchTerm}
                                    onChange={handleUserSearchChange}
                                />
                                <Button variant="outline-secondary" disabled><BsSearch /></Button>
                            </InputGroup>
                            {(isSearchingUsers || isFetchingUsers) && <Spinner animation="border" size="sm" />}
                            {userSearchResults && userSearchResults.length > 0 && (
                                <ListGroup style={{ maxHeight: '150px', overflowY: 'auto' }} className="mb-2 position-relative">
                                    {userSearchResults
                                        .filter(u => !allowedUsers.some(au => au.id === u.id))
                                        .map(user => (
                                            <ListGroup.Item action onClick={() => handleAddUser(user)} key={user.id} className="py-1 px-2 small d-flex justify-content-between align-items-center">
                                                {user.userName} ({user.email})
                                                <small className='text-primary'>Add</small>
                                            </ListGroup.Item>
                                        ))}
                                </ListGroup>
                            )}
                            <div className='p-2 border rounded mb-2' style={{ minHeight: '60px' }}>
                                {allowedUsers.length > 0 ? (
                                    <div className='d-flex flex-wrap gap-1'>
                                        {allowedUsers.map((user) => (
                                            <Badge pill bg="success" key={user.id} className="d-flex align-items-center">
                                                {user.userName}
                                                <CloseButton className='ms-1' onClick={() => removeUser(user.id)} bsPrefix='btn-close-white btn-close' style={{ fontSize: '0.6em' }} aria-label={`Remove ${user.userName}`} />
                                            </Badge>
                                        ))}
                                    </div>
                                ) : (
                                    <small className="text-muted">{t('templateSettings.noUsersSelected', 'No users selected yet.')}</small>
                                )}
                            </div>
                            <Form.Text muted>{t('createTemplate.hints.allowedUsers')}</Form.Text>
                            {allowedUsers.length === 0 && !watchedIsPublic && <div className="text-danger small mt-1">{t('createTemplate.errors.allowedUsersRequired')}</div>}
                        </Form.Group>
                    )}
                    <Button type="submit" variant="primary" size="sm" disabled={isUpdatingAccess || !accessChanged}>
                        {isUpdatingAccess ? <Spinner as="span" animation="border" size="sm" /> : t('templateSettings.buttons.saveAccess')}
                    </Button>
                </Form>
            </Card.Body>
        </Card>
    );
};

export default TemplateSettings;