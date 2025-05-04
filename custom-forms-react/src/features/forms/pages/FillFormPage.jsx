import React, { useState } from 'react'; 
import { useParams, useNavigate } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { Container, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useGetTemplateByIdQuery } from '../../../app/api/templatesApi';
import { useSubmitFormMutation } from '../../../app/api/formsApi';

const FillFormPage = () => {
    const { templateId } = useParams();
    const navigate = useNavigate();

    const { data: template, error: templateError, isLoading: isLoadingTemplate } = useGetTemplateByIdQuery(templateId);
    const [submitForm, { isLoading: isSubmitting }] = useSubmitFormMutation();
    const [apiError, setApiError] = useState(null);

    const { register, handleSubmit, control, formState: { errors } } = useForm(); 

    const onSubmit = async (formData) => {
        setApiError(null);
        const answers = template.questions.map(q => ({
            questionId: q.id,
            value: typeof formData[q.id] === 'boolean' ? String(formData[q.id]) : (formData[q.id]?.toString() ?? ''),
        }));

        const submissionData = { templateId, answers };

        try {
            await submitForm(submissionData).unwrap();
            toast.success("Form submitted successfully!");
            navigate('/profile'); 
        } catch (err) {
            console.error('Failed to submit form:', err);
            setApiError(err?.data?.message || err?.error || 'Could not submit form.');
        }
    };

    if (isLoadingTemplate) return <div className="text-center p-5"><Spinner animation="border" /></div>;
    if (templateError || !template) return <Alert variant="danger">Could not load the form template. {JSON.stringify(templateError)} </Alert>;
    if (!template.canCurrentUserFill && !template.canCurrentUserManage) { 
        return <Alert variant="warning">You do not have permission to fill this form.</Alert>;
    }

    return (
        <Container style={{ maxWidth: '650px' }} className="my-4">
            <h2 className="mb-2">{template.title}</h2>
            <p className="text-muted mb-4">{template.description}</p>

            <Card className='shadow-sm'>
                <Card.Body>
                    <Form onSubmit={handleSubmit(onSubmit)}>
                        {apiError && <Alert variant="danger">{apiError}</Alert>}

                        {template.questions?.map(question => (
                            <Form.Group className="mb-4" controlId={`question-${question.id}`} key={question.id}>
                                <Form.Label className="fw-bold">{question.title} {true ? <span className="text-danger">*</span> : ''}</Form.Label>
                                {question.description && <div className="text-muted small mb-2">{question.description}</div>}

                                {question.type === 'String' && (
                                    <Form.Control
                                        type="text"
                                        isInvalid={!!errors[question.id]}
                                        {...register(question.id, { required: `${question.title} is required` })}
                                    />
                                )}
                                {question.type === 'Text' && (
                                    <Form.Control
                                        as="textarea"
                                        rows={3}
                                        isInvalid={!!errors[question.id]}
                                        {...register(question.id, { required: `${question.title} is required` })}
                                    />
                                )}
                                {question.type === 'Integer' && (
                                    <Form.Control
                                        type="number"
                                        isInvalid={!!errors[question.id]}
                                        {...register(question.id, {
                                            required: `${question.title} is required`,
                                            valueAsNumber: true,
                                            min: { value: 0, message: 'Value must be zero or positive' } 
                                        })}
                                    />
                                )}
                                {question.type === 'Checkbox' && (
                                    <Controller
                                        name={question.id}
                                        control={control}
                                        render={({ field: { onChange, onBlur, value, ref } }) => (
                                            <Form.Check
                                                type="checkbox"
                                                id={`question-${question.id}`}
                                                checked={!!value} 
                                                onChange={(e) => onChange(e.target.checked)} 
                                                onBlur={onBlur}
                                                ref={ref}
                                                isInvalid={!!errors[question.id]}
                                            />
                                        )}
                                    />
                                )}

                                <Form.Control.Feedback type="invalid">
                                    {errors[question.id]?.message}
                                </Form.Control.Feedback>
                            </Form.Group>
                        ))}

                        <div className="d-grid">
                            <Button variant="primary" type="submit" disabled={isSubmitting}>
                                {isSubmitting ? <Spinner animation="border" size="sm" /> : 'Submit Answers'}
                            </Button>
                        </div>
                    </Form>
                </Card.Body>
            </Card>
        </Container>
    );
};

export default FillFormPage;