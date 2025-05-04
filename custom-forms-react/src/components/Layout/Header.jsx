import React, { useState } from 'react';
import { LinkContainer } from 'react-router-bootstrap';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import Navbar from 'react-bootstrap/Navbar';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Image from 'react-bootstrap/Image'; 
import { useAuth } from '../../hooks/useAuth';
import { useAppDispatch } from '../../app/hooks';
import { logOut } from '../../features/auth/authSlice';
import LanguageSwitcher from '../Common/LanguageSwitcher'; 
import { BsSunFill, BsMoonStarsFill, BsSearch, BsPersonCircle, BsPlusCircle } from 'react-icons/bs'; 

const Header = ({ currentTheme, onToggleTheme }) => {
    const { t } = useTranslation();
    const { isAuthenticated, user } = useAuth();
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const [searchTerm, setSearchTerm] = useState('');

    const handleLogout = () => {
        dispatch(logOut());
        navigate('/');
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (searchTerm.trim()) {
            navigate(`/search?query=${encodeURIComponent(searchTerm.trim())}`);
            setSearchTerm('');
        }
    };

    const isAdmin = user?.roles?.includes('Admin');

    return (
        <Navbar expand="lg" bg={currentTheme} data-bs-theme={currentTheme} sticky="top" className="shadow-sm border-bottom">
            <Container fluid="xl">
                <LinkContainer to="/">
                    <Navbar.Brand href="#">{t('header.title', 'Custom Forms')}</Navbar.Brand>
                </LinkContainer>
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                <Navbar.Collapse id="basic-navbar-nav">
                    <Form onSubmit={handleSearchSubmit} className="d-flex mx-lg-auto my-2 my-lg-0" style={{ maxWidth: "400px" }}>
                        <InputGroup size="sm">
                            <Form.Control
                                type="search"
                                placeholder={t('header.searchPlaceholder', 'Search templates...')}
                                aria-label="Search"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                            <Button variant="outline-secondary" type="submit" size="sm" aria-label="Search Button"><BsSearch /></Button>
                        </InputGroup>
                    </Form>

                    <Nav className="ms-auto align-items-center">
                        <LanguageSwitcher className="me-2" />

                        <Button
                            variant="outline-secondary"
                            size="sm"
                            onClick={onToggleTheme}
                            className="me-2 d-flex align-items-center" 
                            aria-label={currentTheme === 'light' ? 'Switch to dark mode' : 'Switch to light mode'}
                        >
                            {currentTheme === 'light' ? <BsMoonStarsFill /> : <BsSunFill />}
                        </Button>

                        {isAuthenticated && (
                            <LinkContainer to="/templates/new">
                                <Button variant="success" size="sm" className="me-2 d-flex align-items-center">
                                    <BsPlusCircle className="me-1" /> {t('header.createTemplate', 'Create')}
                                </Button>
                            </LinkContainer>
                        )}

                        {isAuthenticated && user ? (
                            <NavDropdown 
                                title={
                                    user.avatarUrl ? ( 
                                        <Image src={user.avatarUrl} roundedCircle width={24} height={24} className="me-1" />
                                    ) : (
                                        <BsPersonCircle className="me-1" size={20} /> 
                                    )
                                }
                                id="user-dropdown"
                                style={{ zIndex: 1050 }}
                                align="end"
                            >
                                <NavDropdown.Header className="small">{user.userName}</NavDropdown.Header>
                                <LinkContainer to="/profile">
                                    <NavDropdown.Item>{t('header.profile', 'Profile')}</NavDropdown.Item>
                                </LinkContainer>
                                {isAdmin && (
                                    <LinkContainer to="/admin/users">
                                        <NavDropdown.Item>{t('header.admin', 'Admin Panel')}</NavDropdown.Item>
                                    </LinkContainer>
                                )}
                                <NavDropdown.Divider />
                                <NavDropdown.Item onClick={handleLogout}>{t('header.logout', 'Logout')}</NavDropdown.Item>
                            </NavDropdown>
                        ) : (
                            <>
                                <LinkContainer to="/login">
                                    <Nav.Link className="me-1">{t('header.login', 'Login')}</Nav.Link>
                                </LinkContainer>
                                <LinkContainer to="/register">
                                    <Nav.Link as="span">
                                        <Button variant='outline-primary' size='sm'>{t('header.register', 'Register')}</Button>
                                    </Nav.Link>
                                </LinkContainer>
                            </>
                        )}
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};

export default Header;