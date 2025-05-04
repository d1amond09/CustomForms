import React, { useEffect } from 'react';
import Container from 'react-bootstrap/Container';
import Header from './Header';
import { useTranslation } from 'react-i18next';

const Layout = ({ children, currentTheme, onToggleTheme }) => {
  

    return (
        <div className="d-flex flex-column min-vh-100">
            <Header currentTheme={currentTheme} onToggleTheme={onToggleTheme} />
            <main className="flex-grow-1 py-4">
                <Container fluid="xl">
                    {children}
                </Container>
            </main>
            <footer className="mt-auto py-3 bg-body-tertiary border-top">
                <Container fluid="xl">
                    <span className="text-body-secondary"> {new Date().getFullYear()} - Custom Forms App</span>
                </Container>
            </footer>
        </div>
    );
};

export default Layout;