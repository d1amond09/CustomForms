import React, { useEffect } from 'react';
import { Modal, Button, Form, Spinner, Alert } from 'react-bootstrap';
import { useForm, Controller } from 'react-hook-form';
import { toast } from 'react-toastify';
import { useAddQuestionMutation, useUpdateQuestionMutation } from '../../../app/api/templatesApi';
import { useTranslation } from 'react-i18next';

const QuestionFormModal = ({ show, handleClose, templateId, questionData = null, onSave }) => {
    const isEditing = !!questionData;
    const questionId = questionData?.id;
    const { t } = useTranslation();

    const [addQuestion, { isLoading: isAdding, error: addError }] = useAddQuestionMutation();
    const [updateQuestion, { isLoading: isUpdating, error: updateError }] = useUpdateQuestionMutation();

    const isLoading = isAdding || isUpdating;
    const mutationError = addError || updateError;

    const { register, handleSubmit, control, reset, formState: { errors } } = useForm({
        defaultValues: {
            title: '',
            description: '',
            type: 'String', 
            showInResults: true,
        }
    });

    useEffect(() => {
        if (isEditing && questionData) {
            reset({
                title: questionData.title,
                description: questionData.description,
                type: questionData.type,
                showInResults: questionData.showInResults,
            });
        } else {
            reset({ title: '', description: '', type: 'String', showInResults: true });
        }
    }, [questionData, isEditing, reset, show]);

    const onSubmit = async (data) => {
        try {
            if (isEditing) {
                await updateQuestion({ templateId, questionId, questionData: data }).unwrap();
                toast.success(t('templates.question.saveSuccessUpdate', 'Question updated successfully!'));
            } else {
                await addQuestion({ templateId, questionData: data }).unwrap();
                toast.success(t('templates.question.saveSuccessAdd', 'Question added successfully!'));
            }
            if(onSave) onSave();
        } catch (err) {
            console.error("Save question error:", err);
            toast.error(err?.data?.message || t('templates.question.saveError', 'Failed to save question'));
        }
    };

    return (
        <Modal show={show} onHide={handleClose} centered>
            <Modal.Header closeButton>
                <Modal.Title>{isEditing ? t('templates.question.editTitle', 'Edit Question') : t('templates.question.addTitle', 'Add New Question')}</Modal.Title>
            </Modal.Header>
            <Form onSubmit={handleSubmit(onSubmit)}>
                <Modal.Body>
                     {mutationError && (
                        <Alert variant="danger" className='py-1 px-2 small'>{mutationError?.data?.message || 'An error occurred.'}</Alert>
                     )}

                    <Form.Group className="mb-3" controlId="questionTitle">
                        <Form.Label> {t('templates.question.labels.title', 'Title')}  <span className="text-danger">*</span></Form.Label>
                        <Form.Control
                            type="text"
                            isInvalid={!!errors.title}
                            {...register('title', { required: t('templates.question.errors.titleRequired', 'Title is required') })}
                        />
                        <Form.Control.Feedback type="invalid">{errors.title?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="questionDescription">
                        <Form.Label>{t('templates.question.description', 'Description')} </Form.Label>
                        <Form.Control as="textarea" rows={2} {...register('description')} />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="questionType">
                        <Form.Label>{t('templates.question.labels.type', 'Question Type')} <span className="text-danger">*</span></Form.Label>
                        <Form.Select
                            aria-label="Select question type"
                            isInvalid={!!errors.type}
                            {...register('type', { required: 'Question type is required' })}
                            disabled={isEditing}
                        >
                            <option value="String">{t('templates.question.types.String', 'Single-line text')} </option>
                            <option value="Text">{t('templates.question.types.Text', 'Multi-line text')}</option>
                            <option value="Integer">{t('templates.question.types.Integer', 'Positive Integer')}</option>
                            <option value="Checkbox">{t('templates.question.types.Checkbox', 'Checkbox (Yes/No)')}</option>
                        </Form.Select>
                        {isEditing && <Form.Text muted>{t('templates.question.hints.typeChangeDisabled', 'Question type cannot be changed after creation.')}</Form.Text>}
                        <Form.Control.Feedback type="invalid">{errors.type?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="questionShowInResults">
                       <Controller
                           name="showInResults"
                           control={control}
                           render={({ field }) => (
                             <Form.Check
                                type="switch"
                                id="showInResultsSwitch"
                                   label={t('templates.question.labels.showInResults', 'Show this question in the results table?')}
                                {...field}
                                checked={field.value}
                             />
                           )}
                       />
                    </Form.Group>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        {t('templates.question.cancel', 'Cancel')}
                    </Button>
                    <Button variant="primary" type="submit" disabled={isLoading}>   
                        {isLoading ? <Spinner animation="border" size="sm" /> : (isEditing ? t('templates.question.save', 'Save Changes') : t('templates.question.addTitle', 'Add New Question'))}
                    </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
};

export default QuestionFormModal;