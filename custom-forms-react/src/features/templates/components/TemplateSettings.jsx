import React, { useState, useEffect, useCallback } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Form, Button, Card, Spinner, Alert, Badge, CloseButton, Image as BsImage } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useUpdateTemplateMutation, useSetAccessMutation, useSetTagsMutation } from '../../../app/api/templatesApi';
import { useGetTopicsQuery } from '../../../app/api/topicsApi';
import { useLazyGetUsersQuery } from '../../../app/api/usersApi';
import Select from 'react-select/async';
import _debounce from 'lodash/debounce';
import { useTranslation } from 'react-i18next';

const TemplateSettings = ({ template, onSettingsUpdated }) => {
    const { t } = useTranslation();
    const { id: templateId, title, description, topicId, isPublic, imageUrl: currentImageUrl, tags: currentTags = [], allowedUsers: currentAllowedUsers = [] } = template;

    const { data: topicsData, isLoading: isLoadingTopics, isError: isErrorTopics } = useGetTopicsQuery({ pageNumber: 1, pageSize: 999 });
    const [updateTemplate, { isLoading: isUpdatingMeta, error: metaError }] = useUpdateTemplateMutation();
    const [setAccess, { isLoading: isUpdatingAccess, error: accessError }] = useSetAccessMutation();
    const [saveTags, { isLoading: isUpdatingTags, error: tagsError }] = useSetTagsMutation();
    const [triggerUserSearch, userSearchResult] = useLazyGetUsersQuery();
    const { data: searchResultsData, isLoading: isLoadingUsers, isFetching: isFetchingUsers } = userSearchResult;

    const [tags, setTags] = useState([]);
    const [tagInput, setTagInput] = useState('');
    const [allowedUsers, setAllowedUsers] = useState([]);
    const [apiError, setApiError] = useState(null);

    const [newImageFile, setNewImageFile] = useState(null);
    const [previewNewImage, setPreviewNewImage] = useState(null);
    const [removeCurrentImage, setRemoveCurrentImage] = useState(false);

    const { register, handleSubmit, control, watch, reset, formState: { errors, isDirty: isMetaDirty } } = useForm({
        defaultValues: {
            title: template?.title || '',
            description: template?.description || '',
            topicId: template?.topicId || '',
            isPublic: template?.isPublic ?? true, 
        }
    });
    const watchedIsPublic = watch('isPublic');

    const [tagsChanged, setTagsChanged] = useState(false);
    const [accessChanged, setAccessChanged] = useState(false);

    useEffect(() => {
        if (template) {
            reset({
                title: template.title || '',
                description: template.description || '',
                topicId: template.topicId || '',
                isPublic: template.isPublic ?? true,
            });
            setTags((template.tags || []).map(t => t.name.toLowerCase()).sort());
            setAllowedUsers([...(template.allowedUsers || [])].sort((a, b) => a.id.localeCompare(b.id)));
            setTagsChanged(false);
            setAccessChanged(false);
        }
    }, [template, reset]);

    useEffect(() => {
        if (!template) return;
        const initialAllowedIds = (template.allowedUsers || []).map(u => u.id).sort().join(',');
        const currentAllowedIds = allowedUsers.map(u => u.id).sort().join(',');
        const publicChanged = (template.isPublic ?? true) !== watchedIsPublic;
        const usersChanged = initialAllowedIds !== currentAllowedIds;

        setAccessChanged(publicChanged || (!watchedIsPublic && usersChanged));
    }, [watchedIsPublic, allowedUsers, template]);

    useEffect(() => {
        if (!template) return;
        const initialTagsString = (template.tags || []).map(t => t.name.toLowerCase()).sort().join(',');
        const currentTagsString = [...tags].sort().join(',');
        setTagsChanged(initialTagsString !== currentTagsString);
    }, [tags, template]);


    const handleNewFileChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            setNewImageFile(file);
            setPreviewNewImage(URL.createObjectURL(file));
            setRemoveCurrentImage(false);
        } else {
            setNewImageFile(null);
            setPreviewNewImage(null);
        }
    };

    const handleRemoveCurrentImageChange = (e) => {
        setRemoveCurrentImage(e.target.checked);
        if (e.target.checked) {
            setNewImageFile(null); 
            setPreviewNewImage(null);
        }
    };

    const handleTagInputKeyDown = (e) => {
        if ((e.key === ',' || e.key === 'Enter') && tagInput.trim()) {
            e.preventDefault();
            const newTag = tagInput.trim().toLowerCase();
            if (newTag && !tags.includes(newTag)) {
                setTags([...tags, newTag].sort());
            }
            setTagInput('');
        }
    };

    const removeTag = (tagToRemove) => {
        setTags(tags.filter(tag => tag !== tagToRemove));
    };

    const loadUserOptions = useCallback(
        _debounce((inputValue, callback) => {
            if (!inputValue || inputValue.length < 2) {
                callback([]);
                return;
            }
            triggerUserSearch({ searchTerm: inputValue, pageSize: 10 })
                .then(result => {
                    const users = result.data?.items || [];
                    const options = users
                        .filter(user => !allowedUsers.some(allowed => allowed.id === user.id))
                        .map(user => ({
                            value: user.id,
                            label: `${user.userName} (${user.email || 'No email'})`,
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
            if (!allowedUsers.some(u => u.id === selectedOption.userData.id)) {
                setAllowedUsers([...allowedUsers, selectedOption.userData].sort((a, b) => a.id.localeCompare(b.id)));
            }
        }
    };

    const removeUser = (userIdToRemove) => {
        setAllowedUsers(allowedUsers.filter(u => u.id !== userIdToRemove));
    };

    const onMetaSubmit = async (data) => {

        console.log(data);
        setApiError(null);
        const updateData = {
            title: data.title,
            description: data.description,
            topicId: data.topicId,
            newImageFile: newImageFile,        
            removeCurrentImage: removeCurrentImage
        };
        try {
            await updateTemplate({ id: templateId, templateDataWithFile: updateData }).unwrap();
            toast.success(t('templateSettings.updateSuccess.details', 'Details updated'));
            setNewImageFile(null); 
            setPreviewNewImage(null);
            setRemoveCurrentImage(false);
            reset(data, { keepDirty: false, keepValues: true });
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.details', 'Failed to update details');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const onAccessSubmit = async () => {
        setApiError(null);
        const currentIsPublic = watchedIsPublic;
        const accessData = {
            isPublic: currentIsPublic,
            allowedUserIds: !currentIsPublic ? allowedUsers.map(u => u.id) : null
        };

        if (!currentIsPublic && (!accessData.allowedUserIds || accessData.allowedUserIds.length === 0)) {
            const errorMsg = t('createTemplate.errors.allowedUsersRequired');
            setApiError(errorMsg);
            toast.error(errorMsg);
            return;
        }
        try {
            await setAccess({ templateId, accessData }).unwrap();
            toast.success(t('templateSettings.updateSuccess.access', 'Access updated'));
            setAccessChanged(false);
            reset({ ...watch(), isPublic: currentIsPublic }, { keepDirty: false, keepValues: true });
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.access', 'Failed to update access');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const onTagsSubmit = async () => {
        setApiError(null);
        try {
            await saveTags({ templateId, tagNames: tags }).unwrap();
            toast.success(t('templateSettings.updateSuccess.tags', 'Tags updated'));
            setTagsChanged(false);
            if (onSettingsUpdated) onSettingsUpdated();
        } catch (err) {
            const errorMsg = err?.data?.message || t('templateSettings.updateError.tags', 'Failed to update tags');
            setApiError(errorMsg);
            toast.error(errorMsg);
        }
    };

    const topics = topicsData?.items || [];
    const noTopicsAvailable = !isLoadingTopics && !isErrorTopics && topics.length === 0;

    const selectStyles = {
        control: (provided, state) => ({
            ...provided,
            minHeight: '38px',
            boxShadow: state.isFocused ? '0 0 0 0.25rem rgba(13, 110, 253, 0.25)' : 'none',
            borderColor: state.isFocused ? '#86b7fe' : '#dee2e6',
            '&:hover': {
                borderColor: state.isFocused ? '#86b7fe' : '#adb5bd'
            }
        }),
        valueContainer: (provided) => ({
            ...provided,
            padding: '1px 6px'
        }),
        input: (provided) => ({
            ...provided,
            margin: '0px',
            padding: '0px'
        }),
        indicatorSeparator: () => ({
            display: 'none',
        }),
        indicatorsContainer: (provided) => ({
            ...provided,
            padding: '1px'
        }),
        placeholder: (provided) => ({
            ...provided,
            color: '#6c757d'
        }),
        noOptionsMessage: (provided) => ({
            ...provided,
            color: '#6c757d',
            fontSize: '0.9em',
        })
    };

    const handleSelectChange = (selectedOption) => {
        if (selectedOption) {
            handleUserSelect(selectedOption);
        }
    };

    return (
        <Card>
            <Card.Body>
                <Card.Title as="h5" className='mb-3'>{t('templateSettings.title', 'Template Settings')}</Card.Title>
                {apiError && <Alert variant="danger" className="py-1 px-2 small">{apiError}</Alert>}

                <Form onSubmit={handleSubmit(onMetaSubmit)} className="mb-4 border-bottom pb-4">
                    <h6 className='mb-3'>{t('templateSettings.subtitles.general', 'General Information')}</h6>
                    {(metaError) && <Alert variant="danger" size="sm">{metaError?.data?.message || t('templateSettings.updateError.details')}</Alert>}
                    <Form.Group className="mb-3" controlId="templateSettingsTitle">
                        <Form.Label>{t('createTemplate.labels.title')} <span className="text-danger">*</span></Form.Label>
                        <Form.Control
                            type="text"
                            isInvalid={!!errors.title}
                            {...register('title', { required: t('createTemplate.errors.titleRequired') })}
                        />
                        <Form.Control.Feedback type="invalid">{errors.title?.message}</Form.Control.Feedback>
                    </Form.Group>

                    

                    <Form.Group controlId="templateSettingsImage" className="mb-3">
                        <Form.Label>{t('templateSettings.currentImage', 'Current Image')}</Form.Label>
                        {currentImageUrl && !previewNewImage && !removeCurrentImage ? (
                            <div className="mb-2">
                                <BsImage src={currentImageUrl} alt="Current template" thumbnail style={{ maxHeight: '150px' }} />
                            </div>
                        ) : previewNewImage ? (
                            <div className="mb-2">
                                <BsImage src={previewNewImage} alt="New preview" thumbnail style={{ maxHeight: '150px' }} />
                            </div>
                        ) : (
                            <p className="text-muted small">{t('templateSettings.noImage', 'No image uploaded.')}</p>
                        )}
                        <Form.Label className="mt-2">{t('templateSettings.uploadNew', 'Upload New Image')}</Form.Label> 
                        <Form.Control type="file" accept="image/*" onChange={handleNewFileChange} /> 

                        {currentImageUrl && (
                            <Form.Check
                                type="checkbox"
                                label={t('templateSettings.removeCurrentImage', 'Remove current image')}
                                checked={removeCurrentImage}
                                onChange={handleRemoveCurrentImageChange}
                                className="mt-2"
                                id="removeCurrentImageCheckboxInternal" 
                            />
                        )}
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="templateSettingsDescription">
                        <Form.Label>{t('createTemplate.labels.description')}</Form.Label>
                        <Form.Control as="textarea" rows={3} {...register('description')} />
                        <Form.Text muted>{t('createTemplate.hints.markdown')}</Form.Text>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="templateSettingsTopic">
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
                    {(tagsError) && <Alert variant="danger" size="sm">{tagsError?.data?.message || t('templateSettings.updateError.tags', 'Error updating tags')}</Alert>}
                    <Form.Group className="mb-3" controlId="templateSettingsTagInput">
                        <Form.Label>{t('createTemplate.labels.tags')}</Form.Label>
                        <Form.Control
                            type="text"
                            value={tagInput}
                            onChange={(e) => setTagInput(e.target.value)}
                            onKeyDown={handleTagInputKeyDown}
                            placeholder={t('createTemplate.placeholders.tags', 'Type tag and press Enter or Comma...')}
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
                        {isUpdatingTags ? <Spinner as="span" animation="border" size="sm" /> : t('templateSettings.buttons.saveTags', 'Save Tags')}
                    </Button>
                </Form>

                <Form onSubmit={(e) => { e.preventDefault(); onAccessSubmit(); }}>
                    <h6 className='mb-3'>{t('templateSettings.subtitles.access', 'Access Control')}</h6>
                    {(accessError) && <Alert variant="danger" size="sm">{accessError?.data?.message || t('templateSettings.updateError.access', 'Error updating access')}</Alert>}

                    <Form.Group className="mb-3" controlId="templateSettingsIsPublic">
                        <Controller
                            name="isPublic"
                            control={control}
                            render={({ field: { onChange, onBlur, value, ref } }) => (
                                <Form.Check
                                    type="switch"
                                    id="settingsIsPublicSwitch"
                                    label={t('createTemplate.labels.isPublic')}
                                    checked={value}
                                    onChange={(e) => onChange(e.target.checked)}
                                    onBlur={onBlur}
                                    ref={ref}
                                />
                            )}
                        />
                    </Form.Group>

                    {!watchedIsPublic && (
                        <Form.Group className="mb-3" controlId="templateSettingsAllowedUsers">
                            <Form.Label>{t('createTemplate.labels.allowedUsers')} <span className="text-danger">*</span></Form.Label>
                            <Select
                                id="user-select-async"
                                instanceId="user-select-async-instance"
                                cacheOptions
                                defaultOptions
                                loadOptions={loadUserOptions}
                                onChange={handleSelectChange}
                                isLoading={isLoadingUsers || isFetchingUsers}
                                isClearable={false}
                                placeholder={t('createTemplate.placeholders.userSearch', 'Search user by name or email...')}
                                noOptionsMessage={({ inputValue }) =>
                                    !inputValue || inputValue.length < 2
                                        ? t('templateSettings.userSearch.prompt', "Type 2+ chars to search")
                                        : t('templateSettings.userSearch.noResults', "No users found")
                                }
                                styles={selectStyles}
                                classNamePrefix="react-select"
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
                                            style={{ fontSize: '0.65em', boxShadow: 'none', filter: 'invert(1) grayscale(100%) brightness(200%)' }}
                                        />
                                    </Badge>
                                ))}
                            </div>
                            <Form.Text muted>{t('createTemplate.hints.allowedUsers')}</Form.Text>
                            {allowedUsers.length === 0 && <div className="text-danger small mt-1">{t('createTemplate.errors.allowedUsersRequired')}</div>}
                        </Form.Group>
                    )}

                    <Button type="submit" variant="primary" size="sm" disabled={isUpdatingAccess || !accessChanged}>
                        {isUpdatingAccess ? <Spinner as="span" animation="border" size="sm" /> : t('templateSettings.buttons.saveAccess', 'Save Access')}
                    </Button>
                </Form>
            </Card.Body>
        </Card>
    );
};

export default TemplateSettings;