import React, { useState } from 'react'; 
import { Link as RouterLink, useNavigate, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Container, Row, Col, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { useLoginMutation } from '../../../app/api/authApi';
import { useTranslation } from 'react-i18next';
import { toast } from 'react-toastify'; 

const LoginPage = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();
    const [login, { isLoading }] = useLoginMutation();
    const [apiError, setApiError] = useState(null); 

    const { register, handleSubmit, formState: { errors } } = useForm();

    const from = location.state?.from?.pathname || "/";

    const onSubmit = async (data) => {
        setApiError(null); 
        try {
            await login(data).unwrap(); 
            navigate(from, { replace: true });
        } catch (err) {
            const errorMsg = err?.data?.message || err?.error || 'Invalid credentials or server error.';
            setApiError(errorMsg); 
            console.error('Failed to login:', err);
        }
    };

    return (
        <Container style={{ maxWidth: '450px' }} className="mt-5">
            <h2 className="text-center mb-4">{t('loginPage.title', 'Login')}</h2>
            <Card className="p-4 shadow-sm">
                <Card.Body>
                    <Form onSubmit={handleSubmit(onSubmit)}>
                        {apiError && <Alert variant="danger" className='py-2 px-3 small'>{apiError}</Alert>}

                        <Form.Group className="mb-3" controlId="loginUserName">
                            <Form.Label>{t('loginPage.usernameLabel', 'Username')}</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Enter username"
                                isInvalid={!!errors.userName} 
                                {...register('userName', { required: 'Username is required' })}
                            />
                            <Form.Control.Feedback type="invalid">
                                {errors.userName?.message}
                            </Form.Control.Feedback>
                        </Form.Group>

                        <Form.Group className="mb-3" controlId="loginPassword">
                            <Form.Label>{t('loginPage.passwordLabel', 'Password')}</Form.Label>
                            <Form.Control
                                type="password"
                                placeholder="Password"
                                isInvalid={!!errors.password}
                                {...register('password', { required: 'Password is required' })}
                            />
                            <Form.Control.Feedback type="invalid">
                                {errors.password?.message}
                            </Form.Control.Feedback>
                        </Form.Group>

                        <div className="d-grid"> 
                            <Button variant="primary" type="submit" disabled={isLoading}>
                                {isLoading ? <Spinner animation="border" size="sm" /> : t('loginPage.submitButton', 'Log In')}
                            </Button>
                        </div>

                        <div className="text-center mt-3">
                            <small className="text-muted">
                                {t('loginPage.registerPrompt', "Don't have an account?")}{' '}
                                <RouterLink to="/register" className='text-decoration-none'>{t('loginPage.registerLink', 'Register here')}</RouterLink>
                            </small>
                        </div>
                    </Form>
                </Card.Body>
            </Card>
        </Container>
    );
};

export default LoginPage;