import React, { useState } from 'react';
import { Card, Form, Button, Stack, Spinner, Alert, CloseButton } from 'react-bootstrap';
import { BsTrash } from 'react-icons/bs';

const CommentSection = ({ comments = [], isLoading, isAddingComment, onAddComment, onDeleteComment, currentUserId, isAdmin, isAuthenticated }) => {
    const [newComment, setNewComment] = useState('');
    const [addError, setAddError] = useState(''); 

    const handleAddClick = async () => {
        if (!newComment.trim() || !onAddComment) return;
        setAddError('');
        try {
            await onAddComment(newComment); 
            setNewComment(''); 
        } catch (error) {
            setAddError(error?.data?.message || error?.message || "Failed to add comment");
        }
    };

    return (
        <Card className="mt-4 shadow-sm">
            <Card.Body>
                <Card.Title as="h5">Comments ({comments.length})</Card.Title>

                {isAuthenticated && ( 
                    <Form className="my-3">
                        {addError && <Alert variant="danger" className='py-1 px-2 small'>{addError}</Alert>}
                        <Form.Group controlId="newCommentTextarea">
                            <Form.Control
                                as="textarea"
                                rows={2}
                                placeholder="Add your comment..."
                                value={newComment}
                                onChange={(e) => setNewComment(e.target.value)}
                                disabled={isAddingComment}
                            />
                        </Form.Group>
                        <Button
                            variant="primary"
                            size="sm"
                            className="mt-2"
                            onClick={handleAddClick}
                            disabled={isAddingComment || !newComment.trim()}
                        >
                            {isAddingComment ? <Spinner animation="border" size="sm" /> : 'Post Comment'}
                        </Button>
                    </Form>
                )}
                {!isAuthenticated && <p className='text-muted small my-3'>Please log in to add comments.</p>}

                <div className="mt-4"> 
                    {isLoading && <div className="text-center"><Spinner animation="border" /></div>}
                    {!isLoading && comments.length === 0 && <p className='text-muted'>No comments yet.</p>}

                    <Stack gap={3}>
                        {comments.map(comment => {
                            const canDelete = isAdmin || comment.userId === currentUserId;
                            return (
                                <div key={comment.id} className="border-bottom pb-2">
                                    <div className="d-flex justify-content-between align-items-center mb-1">
                                        <span className="fw-bold small">{comment.userName || 'Anonymous'}</span>
                                        <small className="text-muted">{new Date(comment.createdDate).toLocaleString()}</small>
                                    </div>
                                    <div className="d-flex justify-content-between align-items-start">
                                        <p className="mb-0 small flex-grow-1 me-2">{comment.text}</p>
                                        {canDelete && onDeleteComment && (
                                            <Button
                                                variant="link"
                                                size="sm"
                                                className="text-danger p-0"
                                                onClick={() => onDeleteComment(comment.id)}
                                                title="Delete comment"
                                            >
                                                <BsTrash />
                                            </Button>
                                        )}
                                    </div>
                                </div>
                            );
                        })}
                    </Stack>
                </div>
            </Card.Body>
        </Card>
    );
};

export default CommentSection;