import { Link as RouterLink } from 'react-router-dom';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';

const NotFoundPage = () => {
    return (
        <Container className="d-flex flex-column justify-content-center align-items-center text-center" style={{ minHeight: '60vh' }}>
            <h1>404 - Page Not Found</h1>
            <p className="lead text-muted my-3">
                Sorry, the page you are looking for does not exist.
            </p>
            <Button as={RouterLink} to="/" variant="primary">
                Go to Homepage
            </Button>
        </Container>
    );
};

export default NotFoundPage;