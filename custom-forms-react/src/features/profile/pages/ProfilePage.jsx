import React from 'react';
import { Container, Row, Col, Card, Tabs, Tab, Spinner, Alert } from 'react-bootstrap';
import { useAuth } from '../../../hooks/useAuth';
import UserTemplateTable from '../components/UserTemplateTable'; 
import UserFormTable from '../components/UserFormTable';       
import { useTranslation } from 'react-i18next'; 

const ProfilePage = () => {
    const { t } = useTranslation();
    const { user, isAuthenticated } = useAuth();

    if (!isAuthenticated || !user) {
        return <Container className='text-center mt-5'><Alert variant="warning">{t('profile.loginRequired', 'Please log in to view your profile.')}</Alert></Container>;
    }

    return (
        <Container fluid="xl" className="my-4">
            <h2 className="mb-4">{t('profile.title', 'My Profile')}</h2>
            <Row>
                <Col md={4} className="mb-4 mb-md-0">
                    <Card className='shadow-sm'>
                        <Card.Body>
                            <Card.Title>{t('profile.userInfoTitle', 'User Information')}</Card.Title>
                            <p className='mb-1'><strong>{t('profile.username', 'Username:')}</strong></p>
                            <p className='text-muted'>{user.userName}</p>
                            <p className='mb-1'><strong>{t('profile.email', 'Email:')}</strong></p>
                            <p className='text-muted'>{user.email}</p>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={8}>
                    <Tabs defaultActiveKey="templates" id="profile-tabs" className="mb-3 profile-tabs custom-profile-tabs" fill>
                        <Tab eventKey="templates" title={t('profile.tabTemplates', 'My Templates')}>
                            <div className="mt-3">
                                <UserTemplateTable userId={user.id} />
                            </div>
                        </Tab>
                        <Tab eventKey="forms" title={t('profile.tabForms', 'My Submitted Forms')}>
                            <div className="mt-3">
                                <UserFormTable userId={user.id} />
                            </div>
                        </Tab>
                    </Tabs>
                </Col>
            </Row>
        </Container>
    );
};

export default ProfilePage;