import React, { useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Modal, Form, Button, Spinner, Alert } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useLocation } from 'react-router-dom';
import { useCreateSupportTicketMutation } from '../../app/api/supportApi';


const SupportTicketModal = ({ show, handleClose, templateTitle }) => {
    const location = useLocation();
    const [createSupportTicket, { isLoading }] = useCreateSupportTicketMutation();
    const [apiError, setApiError] = useState(null);

    const { register, handleSubmit, control, reset, formState: { errors } } = useForm({
        defaultValues: {
            summary: '',
            priority: 'Average'
        }
    });

    const onSubmit = async (data) => {
        setApiError(null);
        const ticketData = {
            ...data,
            pageUrl: window.location.href, // Current page URL
            templateTitle: templateTitle || null // Pass template title if available
        };

        try {
            const result = await createSupportTicket(ticketData).unwrap();
            if (result && (result.success !== false)) { // Handle cases where success might be undefined on true success
                toast.success("Support ticket submitted successfully! We'll get back to you soon.");
                reset(); // Clear the form
                handleClose(); // Close the modal
            } else {
                const errMsg = result?.errorMessage || "Failed to submit ticket. Please try again.";
                setApiError(errMsg);
                toast.error(`Submission Failed: ${errMsg}`);
            }
        } catch (err) {
            const errMsg = err?.data?.message || err?.message || "An error occurred while submitting the ticket.";
            setApiError(errMsg);
            toast.error(`Submission Failed: ${errMsg}`);
            console.error("Support ticket submission error:", err);
        }
    };

    const onModalClose = () => {
        reset(); // Reset form when modal is closed
        setApiError(null);
        handleClose();
    }

    return (
        <Modal show={show} onHide={onModalClose} centered>
            <Modal.Header closeButton>
                <Modal.Title>Create Support Ticket</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {apiError && <Alert variant="danger" className="py-2 small">{apiError}</Alert>}
                <Form onSubmit={handleSubmit(onSubmit)}>
                    <Form.Group className="mb-3" controlId="ticketSummary">
                        <Form.Label>Summary <span className="text-danger">*</span></Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={4}
                            placeholder="Please describe your issue or question..."
                            isInvalid={!!errors.summary}
                            {...register("summary", { required: "Summary is required." })}
                        />
                        <Form.Control.Feedback type="invalid">{errors.summary?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="ticketPriority">
                        <Form.Label>Priority <span className="text-danger">*</span></Form.Label>
                        <Controller
                            name="priority"
                            control={control}
                            rules={{ required: "Priority is required." }}
                            render={({ field }) => (
                                <Form.Select {...field} isInvalid={!!errors.priority}>
                                    <option value="Average">Average</option>
                                    <option value="Low">Low</option>
                                    <option value="High">High</option>
                                </Form.Select>
                            )}
                        />
                        <Form.Control.Feedback type="invalid">{errors.priority?.message}</Form.Control.Feedback>
                    </Form.Group>

                    <p className="small text-muted">
                        The current page link ({location.pathname})
                        {templateTitle && ` and template title "${templateTitle}"`} will be automatically included.
                    </p>

                    <div className="d-flex justify-content-end gap-2 mt-4">
                        <Button variant="outline-secondary" onClick={onModalClose} disabled={isLoading}>
                            Cancel
                        </Button>
                        <Button variant="primary" type="submit" disabled={isLoading}>
                            {isLoading ? <Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" /> : "Submit Ticket"}
                        </Button>
                    </div>
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default SupportTicketModal;