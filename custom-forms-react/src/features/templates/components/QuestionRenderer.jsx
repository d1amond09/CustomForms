import React from 'react';
import { Card, Button, Stack } from 'react-bootstrap';
import { BsTrash, BsPencil, BsArrowsMove } from 'react-icons/bs';


const QuestionRenderer = ({ question, index, canManage, onEdit, onDelete, isBusy }) => {
    return (
        <Card body className={`mb-0 shadow-sm question-card ${isBusy ? 'opacity-75 pe-none' : ''}`}> {/* Блокируем клики/вид во время операций */}
            <div className="d-flex justify-content-between align-items-start">
                <div className="flex-grow-1">
                    <span className='text-muted me-2 d-inline-block' style={{ cursor: 'grab' }} title="Drag to reorder"><BsArrowsMove /></span>
                    <span className="fw-bold">#{index + 1}: {question.title}</span> <span className='text-muted small'>({question.type})</span>
                    {question.description && <div className="text-muted small mt-1">{question.description}</div>}
                    <small className="d-block text-muted mt-1 fst-italic">Show in results table: {question.showInResults ? 'Yes' : 'No'}</small>
                </div>
                {canManage && (
                    <Stack direction="horizontal" gap={1} className='ms-2'>
                        <Button variant="link" size="sm" className="text-primary p-1" title="Edit" onClick={onEdit} disabled={isBusy}><BsPencil /></Button>
                        <Button variant="link" size="sm" className="text-danger p-1" title="Delete" onClick={onDelete} disabled={isBusy}><BsTrash /></Button>
                    </Stack>
                )}
            </div>
        </Card>
    );
};

export default QuestionRenderer;