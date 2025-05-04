import React, { useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Container, Card, Form, Button, Alert, Spinner, Badge, CloseButton } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useCreateTemplateMutation } from '../../../app/api/templatesApi';
import { useGetTopicsQuery } from '../../../app/api/topicsApi';
import { useLazyGetUsersQuery } from '../../../app/api/usersApi';
import Select from 'react-select/async';
import { useTranslation } from 'react-i18next';
import _debounce from 'lodash/debounce';

const CreateTemplatePage = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [createTemplate, { isLoading: isCreating }] = useCreateTemplateMutation();
    const { data, isLoading: isLoadingTopics, isError: isErrorTopics } = useGetTopicsQuery({ pageNumber: 1, pageSize: 999 });
    const [triggerUserSearch, { data: userSearchResults, isLoading: isLoadingUsers }] = useLazyGetUsersQuery();

    const [tags, setTags] = useState([]);
    const [tagInput, setTagInput] = useState('');
    const [allowedUsers, setAllowedUsers] = useState([]);
    const [apiError, setApiError] = useState(null);

    const { register, handleSubmit, watch, formState: { errors } } = useForm({
        defaultValues: {
            title: '',
            description: '',
            topicId: '',
            isPublic: true,
        }
    });
    const isPublic = watch('isPublic');

    const removeUser = (userIdToRemove) => {
        setAllowedUsers(allowedUsers.filter(u => u.id !== userIdToRemove));
    };

    const loadUserOptions = useCallback(
        _debounce((inputValue, callback) => {
            if (!inputValue || inputValue.length < 2) {
                callback([]);
                return;
            }
            triggerUserSearch({ searchTerm: inputValue, maxResults: 10 })
                .unwrap()
                .then((users) => {
                    const options = users.items
                        .filter(user => !allowedUsers.some(allowed => allowed.id === user.id))
                        .map(user => ({
                            value: user.id,
                            label: user.userName + (user.email ? ` (${user.email})` : ''),
                            userData: { id: user.id, userName: user.userName }
                        }));
                    callback(options);
                })
                .catch(error => {
                    console.error("User search error:", error);
                    callback([]);
                });
        }, 500),
        [triggerUserSearch, allowedUsers]
    );

    const handleUserSelect = (selectedOption) => {
        if (selectedOption?.userData) {
            handleAddUser(selectedOption.userData);
        }
    };

    const handleAddUser = (user) => {
        if (user && !allowedUsers.some(u => u.id === user.id)) {
            setAllowedUsers([...allowedUsers, user]);
        }
    };

    const handleTagInputKeyDown = (e) => {
        if ((e.key === ',' || e.key === 'Enter') && tagInput.trim()) {
            e.preventDefault();
            const newTag = tagInput.trim().toLowerCase();
            if (newTag && !tags.includes(newTag)) {
                setTags([...tags, newTag]);
            }
            setTagInput('');
        }
    };
    const removeTag = (tagToRemove) => {
        setTags(tags.filter(tag => tag !== tagToRemove));
    };

    const onSubmit = async (data) => {
        setApiError(null);
        const templateData = {
            ...data,
            topicId: data.topicId || null,
            tags: tags,
            allowedUserIds: !data.isPublic ? allowedUsers.map(u => u.id) : null,
        };

        if (!templateData.topicId) {
            setApiError(t('createTemplate.errors.topicRequired', "Please select a topic."));
            return;
        }
        if (!data.isPublic && (!templateData.allowedUserIds || templateData.allowedUserIds.length === 0)) {
            setApiError(t('createTemplate.errors.allowedUsersRequired', "Please select allowed users for a restricted template."));
            return;
        }

        try {
            const result = await createTemplate(templateData).unwrap();
            toast.success(t('createTemplate.success', 'Template Created Successfully!'));
            navigate(`/templates/${result}`);
        } catch (err) {
            console.error('Failed to create template:', err);
            const errorMsg = err?.data?.message || err?.error || t('createTemplate.errors.generalFail', 'Could not create template.');
            setApiError(errorMsg);
        }
    };

    const topics = data?.items;
    const noTopicsAvailable = !isLoadingTopics && !isErrorTopics && topics.length === 0;

    const selectStyles = {
        control: (provided) => ({
            ...provided,
            minHeight: '38px',
            boxShadow: 'none',
            '&:hover': {
                borderColor: '#86b7fe'
            }
        }),
        valueContainer: (provided) => ({
            ...provided,
            padding: '0 6px'
        }),
        input: (provided) => ({
            ...provided,
            margin: '0px',
        }),
        indicatorSeparator: () => ({
            display: 'none',
        }),
        indicatorsContainer: (provided) => ({
            ...provided,
            height: '38px',
        }),
    };

    return (
        <Container style={{ maxWidth: '700px' }} className="my-4">
            <h2 className="mb-4">{t('createTemplate.title', 'Create New Template')}</h2>
            <Card className='shadow-sm'>
                <Card.Body>
                    <Form onSubmit={handleSubmit(onSubmit)}>
                        {apiError && <Alert variant="danger">{apiError}</Alert>}
                        <Form.Group className="mb-3" controlId="templateTitle">
                            <Form.Label>{t('createTemplate.labels.title', 'Title')} <span className="text-danger">*</span></Form.Label>
                            <Form.Control
                                type="text"
                                isInvalid={!!errors.title}
                                {...register('title', { required: t('createTemplate.errors.titleRequired', 'Title is required') })}
                            />
                            <Form.Control.Feedback type="invalid">{errors.title?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="templateDescription">
                            <Form.Label>{t('createTemplate.labels.description', 'Description')}</Form.Label>
                            <Form.Control as="textarea" rows={3} {...register('description')} />
                            <Form.Text muted>{t('createTemplate.hints.markdown', 'Markdown is supported (basic).')}</Form.Text>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="templateTopic">
                            <Form.Label>{t('createTemplate.labels.topic', 'Topic')} <span className="text-danger">*</span></Form.Label>
                            <Form.Select
                                aria-label={t('createTemplate.labels.selectTopic', 'Select topic')}
                                isInvalid={!!errors.topicId}
                                {...register('topicId', { required: t('createTemplate.errors.topicRequired', 'Topic is required') })}
                                disabled={isLoadingTopics || noTopicsAvailable}
                            >
                                <option value="">-- {t('createTemplate.labels.selectTopic', 'Select Topic')} --</option>
                                {isLoadingTopics && <option>{t('createTemplate.loading', 'Loading...')}</option>}
                                {isErrorTopics && <option disabled>{t('createTemplate.errors.topicLoadFail', 'Error loading topics')}</option>}
                                {!isLoadingTopics && !isErrorTopics && topics.map(topic => (
                                    <option key={topic.id} value={topic.id}>{topic.name}</option>
                                ))}
                            </Form.Select>
                            {noTopicsAvailable && (
                                <Alert variant='warning' className='mt-2 py-1 px-2 small'>
                                    {t('createTemplate.errors.noTopics', 'No topics available. Please contact an administrator to add topics.')}
                                </Alert>
                            )}
                            <Form.Control.Feedback type="invalid">{errors.topicId?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="templateTags">
                            <Form.Label>{t('createTemplate.labels.tags', 'Tags')}</Form.Label>
                            <Form.Control
                                type="text"
                                value={tagInput}
                                onChange={(e) => setTagInput(e.target.value)}
                                onKeyDown={handleTagInputKeyDown}
                                placeholder={t('createTemplate.placeholders.tags', 'Type a tag and press Enter or Comma...')}
                            />
                            <div className='mt-2 d-flex flex-wrap gap-1'>
                                {tags.map((tag, index) => (
                                    <Badge pill bg="secondary" key={index} className="d-flex align-items-center">
                                        {tag}
                                        <CloseButton className='ms-1' onClick={() => removeTag(tag)} bsPrefix='btn-close-white btn-close' style={{ fontSize: '0.6em' }} />
                                    </Badge>
                                ))}
                            </div>
                            <Form.Text muted>{t('createTemplate.hints.tags', 'Separate tags with comma or Enter.')}</Form.Text>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="templateIsPublic">
                            <Form.Check
                                type="switch"
                                id="isPublicSwitch"
                                label={t('createTemplate.labels.isPublic', 'Public Template (accessible to all logged-in users)')}
                                {...register('isPublic')}
                            />
                        </Form.Group>
                        {!isPublic && (
                            <Form.Group className="mb-3" controlId="templateAllowedUsers">
                                <Form.Label>{t('createTemplate.labels.allowedUsers', 'Allowed Users')} <span className="text-danger">*</span></Form.Label>
                                <Select
                                    cacheOptions
                                    loadOptions={loadUserOptions}
                                    onChange={handleUserSelect}
                                    isLoading={isLoadingUsers}
                                    isClearable={false}
                                    placeholder={t('createTemplate.placeholders.userSearch', 'Search user by name or email...')}
                                    className="select"
                                    value={null}
                                    noOptionsMessage={({ inputValue }) =>
                                        !inputValue || inputValue.length < 2 ? "Type 2+ chars to search" : "No users found"
                                    }
                                    styles={selectStyles}
                                />
                                <div className='mt-2 d-flex flex-wrap gap-1'>
                                    {allowedUsers.map((user) => (
                                        <Badge
                                            pill
                                            bg="success"   
                                            key={user.id}
                                            className="d-flex align-items-center pe-1 text-bg-success"
                                        >
                                            {user.userName}
                                            <CloseButton
                                                className='ms-2'
                                                onClick={() => removeUser(user.id)}
                                                aria-label={`Remove ${user.userName}`}
                                                style={{
                                                    fontSize: '0.65em',
                                                    boxShadow: 'none'
                                                }}
                                            />
                                        </Badge>
                                    ))}
                                </div>
                                <Form.Text muted>{t('createTemplate.hints.allowedUsers', 'Select users who can view and fill this restricted template.')}</Form.Text>
                                {allowedUsers.length === 0 && <div className="text-danger small mt-1">{t('createTemplate.errors.allowedUsersRequired', 'At least one user is required for restricted templates.')}</div>}
                            </Form.Group>
                        )}
                        <div className="d-grid">
                            <Button variant="primary" type="submit" disabled={isCreating || noTopicsAvailable}>
                                {isCreating ? <Spinner animation="border" size="sm" /> : t('createTemplate.submitButton', 'Create Template')}
                            </Button>
                        </div>
                    </Form>
                </Card.Body>    
            </Card>
        </Container>
    );
};

export default CreateTemplatePage;