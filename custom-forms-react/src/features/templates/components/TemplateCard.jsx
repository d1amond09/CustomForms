import { Card, Badge, Stack } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { BsChatSquareDots, BsStar, BsWindow } from 'react-icons/bs';

const TemplateCard = ({ template }) => {
    return (
        <Card className="h-100 shadow-sm template-card">
            <Card.Body className="d-flex flex-column">
                <div className="d-flex justify-content-between align-items-start mb-2">
                    <Badge bg="secondary">{template.topicName || 'No Topic'}</Badge>
                    <Badge bg={template.isPublic ? 'success-subtle' : 'warning-subtle'} text={template.isPublic ? 'success-emphasis' : 'warning-emphasis'}>
                        {template.isPublic ? 'Public' : 'Restricted'}
                    </Badge>
                </div>
                <LinkContainer to={`/templates/${template.id}`} style={{ cursor: 'pointer' }}>
                    <Card.Title as="h6" className="mb-1 text-primary text-decoration-none stretched-link">{template.title}</Card.Title>
                </LinkContainer>
                <Card.Subtitle className="mb-2 text-muted small">
                    By {template.authorName || 'Unknown'} on {new Date(template.createdDate).toLocaleDateString()}
                </Card.Subtitle>
                <div className="mt-auto d-flex justify-content-end small text-body-secondary gap-3 pt-2">
                    <span className="d-flex align-items-center" title="Responses">
                        <BsWindow className="me-1" /> {template.formCount ?? 0}
                    </span>
                    <span className="d-flex align-items-center" title="Responses">
                        <BsChatSquareDots className="me-1" /> {template.commentCount ?? 0}
                    </span>
                    <span className="d-flex align-items-center" title="Likes">
                        <BsStar className="me-1" /> {template.likeCount ?? 0}
                    </span>
                </div>
            </Card.Body>
            <style>{`
                .text-truncate-2 {
                    overflow: hidden;
                    display: -webkit-box;
                    -webkit-line-clamp: 2;
                    -webkit-box-orient: vertical;
                }
                .template-card .card-title a {
                    color: inherit;
                    text-decoration: none;
                }
                .template-card .card-title a:hover {
                    text-decoration: underline;
                }
                .stretched-link::after {
                    position: absolute;
                    top: 0;
                    right: 0;
                    bottom: 0;
                    left: 0;
                    z-index: 1;
                    content: "";
                }
            `}</style>
        </Card>
    );
};

export default TemplateCard;