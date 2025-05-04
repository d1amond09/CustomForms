import React, { useState } from 'react'; 
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Container, Row, Col, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { useRegisterMutation } from '../../../app/api/authApi';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify';

const RegisterPage = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [registerUser, { isLoading }] = useRegisterMutation();
    const [apiError, setApiError] = useState(null);

    const { register, handleSubmit, watch, formState: { errors } } = useForm();
    const password = watch('password'); 

    const onSubmit = async (data) => {
        setApiError(null);
        const { ...registrationData } = data;
        try {
            await registerUser(registrationData).unwrap(); 
            toast.success('Registration Successful! Please check your email to confirm your account.');
            navigate('/login');
        } catch (err) {
            const errorMessages = err?.data?.errors ? Object.values(err.data.errors).flat().join(' ') : (err?.data?.message || err?.error || 'Registration failed.');
            setApiError(errorMessages);
            console.error('Failed to register:', err);
            console.error(data);
        }
    };

    return (
        <Container style={{ maxWidth: '550px' }} className="mt-5">
            <h2 className="text-center mb-4">{t('registerPage.title', 'Register')}</h2>
            <Card className="p-4 shadow-sm">
                <Card.Body>
                    <Form onSubmit={handleSubmit(onSubmit)}>
                        {apiError && <Alert variant="danger" className='py-2 px-3 small'>{apiError}</Alert>}

                        <Row>
                            <Col md={6}>
                                <Form.Group className="mb-3" controlId="regFirstName">
                                    <Form.Label>{t('registerPage.firstNameLabel', 'First Name (Optional)')}</Form.Label>
                                    <Form.Control type="text" {...register('firstName')} />
                                </Form.Group>
                            </Col>
                            <Col md={6}>
                                <Form.Group className="mb-3" controlId="regLastName">
                                    <Form.Label>{t('registerPage.lastNameLabel', 'Last Name (Optional)')}</Form.Label>
                                    <Form.Control type="text" {...register('lastName')} />
                                </Form.Group>
                            </Col>
                        </Row>

                        <Form.Group className="mb-3" controlId="regUserName">
                            <Form.Label>{t('registerPage.usernameLabel', 'Username')}</Form.Label>
                            <Form.Control
                                type="text"
                                isInvalid={!!errors.userName}
                                {...register('userName', { required: 'Username is required' })}
                            />
                            <Form.Control.Feedback type="invalid">{errors.userName?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="regEmail">
                            <Form.Label>{t('registerPage.emailLabel', 'Email')}</Form.Label>
                            <Form.Control
                                type="email"
                                isInvalid={!!errors.email}
                                {...register('email', {
                                    required: 'Email is required',
                                    pattern: { value: /^\S+@\S+$/i, message: 'Invalid email address' }
                                })}
                            />
                            <Form.Control.Feedback type="invalid">{errors.email?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="regPassword">
                            <Form.Label>{t('registerPage.passwordLabel', 'Password')}</Form.Label>
                            <Form.Control
                                type="password"
                                isInvalid={!!errors.password}
                                {...register('password', {
                                    required: 'Password is required',
                                    minLength: { value: 6, message: 'Password must be at least 6 characters' }
                                })}
                            />
                            <Form.Control.Feedback type="invalid">{errors.password?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="regConfirmPassword">
                            <Form.Label>{t('registerPage.confirmPasswordLabel', 'Confirm Password')}</Form.Label>
                            <Form.Control
                                type="password"
                                isInvalid={!!errors.confirmPassword}
                                {...register('confirmPassword', {
                                    required: 'Please confirm password',
                                    validate: value => value === password || 'Passwords do not match'
                                })}
                            />
                            <Form.Control.Feedback type="invalid">{errors.confirmPassword?.message}</Form.Control.Feedback>
                        </Form.Group>

                        <div className="d-grid">
                            <Button variant="primary" type="submit" disabled={isLoading}>
                                {isLoading ? <Spinner animation="border" size="sm" /> : t('registerPage.submitButton', 'Register')}
                            </Button>
                        </div>

                        <div className="text-center mt-3">
                            <small className="text-muted">
                                {t('registerPage.loginPrompt', 'Already have an account?')}{' '}
                                <RouterLink to="/login" className='text-decoration-none'>{t('registerPage.loginLink', 'Login here')}</RouterLink>
                            </small>
                        </div>
                    </Form>
                </Card.Body>
            </Card>
        </Container>
    );
};

export default RegisterPage;