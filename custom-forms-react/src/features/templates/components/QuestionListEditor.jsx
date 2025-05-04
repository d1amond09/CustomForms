import React, { useState, useEffect } from 'react';
import { Stack, Button, Spinner, Alert } from 'react-bootstrap';
import { DragDropContext, Droppable, Draggable } from '@hello-pangea/dnd';
import { toast } from 'react-toastify';
import { BsPlusCircle, BsTrash, BsPencil, BsArrowsMove } from 'react-icons/bs';
import QuestionRenderer from './QuestionRenderer';
import QuestionFormModal from './QuestionFormModal';
import { useRemoveQuestionMutation, useReorderQuestionsMutation } from '../../../app/api/templatesApi';

const QuestionListEditor = ({ templateId, initialQuestions = [], canManage, onQuestionsUpdated }) => {
    const [questions, setQuestions] = useState(initialQuestions);
    const [showModal, setShowModal] = useState(false);
    const [editingQuestion, setEditingQuestion] = useState(null);

    const [removeQuestion, { isLoading: isDeleting }] = useRemoveQuestionMutation();
    const [reorderQuestions, { isLoading: isReordering }] = useReorderQuestionsMutation();

    useEffect(() => {
        setQuestions(initialQuestions);
    }, [initialQuestions]);

    const handleShowAddModal = () => {
        setEditingQuestion(null);
        setShowModal(true);
    };

    const handleShowEditModal = (question) => {
        setEditingQuestion(question);
        setShowModal(true);
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setEditingQuestion(null);
    };

    const handleModalSave = () => {
        handleCloseModal();
        if (onQuestionsUpdated) {
            onQuestionsUpdated();
        }
    };

    const handleDeleteQuestion = async (questionId) => {
        if (window.confirm('Are you sure you want to delete this question?')) {
            try {
                await removeQuestion({ templateId, questionId }).unwrap();
                toast.success('Question deleted.');
                if (onQuestionsUpdated) onQuestionsUpdated();
            } catch (err) {
                toast.error(err?.data?.message || 'Failed to delete question.');
            }
        }
    };

    const handleOnDragEnd = async (result) => {
        const { source, destination, draggableId } = result;

        if (!destination) {
            return;
        }

        if (
            destination.droppableId === source.droppableId &&
            destination.index === source.index
        ) {
            return;
        }

        const items = Array.from(questions);
        const [reorderedItem] = items.splice(source.index, 1);
        items.splice(destination.index, 0, reorderedItem);

        setQuestions(items);

        const orderedQuestionIds = items.map(q => q.id);

        try {
            await reorderQuestions({ templateId, reorderData: { orderedQuestionIds } }).unwrap();
        } catch (err) {
            toast.error(err?.data?.message || 'Failed to save new order.');
            console.error("Reorder failed:", err);
            setQuestions(initialQuestions);
        }
    };

    const getItemStyle = (isDragging, draggableStyle) => ({
        userSelect: 'none',
        boxShadow: isDragging ? '0 4px 8px rgba(0,0,0,0.1)' : 'none',
        ...draggableStyle,
    });

    const getListStyle = isDraggingOver => ({
        background: isDraggingOver ? '#dee2e6' : 'transparent',
        transition: 'background-color 0.2s ease',
    });

    const isBusy = isDeleting || isReordering;
    const dragDisabled = !canManage || isBusy;

    return (
        <>
            {canManage && (
                <Button variant="success" size="sm" className="mb-3 d-flex align-items-center" onClick={handleShowAddModal}>
                    <BsPlusCircle className="me-1" /> Add Question
                </Button>
            )}

            {questions.length === 0 && <p className='text-muted'>No questions added yet.</p>}

            <DragDropContext onDragEnd={handleOnDragEnd}>
                <Droppable
                    droppableId={`questions-${templateId}`}
                    isDropDisabled={false}
                    isCombineEnabled={false}
                    ignoreContainerClipping={false}
                >
                    {(provided, snapshot) => (
                        <div className="d-flex flex-column gap-2"
                            ref={provided.innerRef}
                            {...provided.droppableProps}
                            style={getListStyle(snapshot.isDraggingOver)}
                        >
                            {questions.map((question, index) => (
                                <Draggable
                                    key={question.id}
                                    draggableId={question.id}
                                    index={index}
                                    isDragDisabled={dragDisabled}
                                >
                                    {(providedDraggable, snapshotDraggable) => (
                                        <div
                                            ref={providedDraggable.innerRef}
                                            {...providedDraggable.draggableProps}
                                            {...providedDraggable.dragHandleProps}
                                            style={getItemStyle(
                                                snapshotDraggable.isDragging,
                                                providedDraggable.draggableProps.style
                                            )}
                                        >
                                            <QuestionRenderer
                                                question={question}
                                                index={index}
                                                canManage={canManage}
                                                onEdit={() => handleShowEditModal(question)}
                                                onDelete={() => handleDeleteQuestion(question.id)}
                                                isBusy={isBusy}
                                            />
                                        </div>
                                    )}
                                </Draggable>
                            ))}
                            {provided.placeholder}
                        </div>
                    )}
                </Droppable>
            </DragDropContext>

            <QuestionFormModal
                show={showModal}
                handleClose={handleCloseModal}
                templateId={templateId}
                questionData={editingQuestion}
                onSave={handleModalSave}
            />
        </>
    );
};

export default QuestionListEditor;