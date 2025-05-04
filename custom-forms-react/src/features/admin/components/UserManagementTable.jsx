import React, { useState } from 'react';
import { Table, Button, Spinner, Alert, Stack, Badge, Dropdown, Modal, Form } from 'react-bootstrap';
import {
    useBlockUserMutation,
    useUnblockUserMutation,
    useDeleteUserMutation,
    useSetUserRoleMutation
} from '../../../app/api/usersApi';
import { BsThreeDotsVertical, BsTrash, BsPersonFillLock, BsPersonFillCheck, BsPersonFillAdd } from 'react-icons/bs';
import { toast } from 'react-toastify';
import { useTranslation } from 'react-i18next';

const UserManagementTable = ({ users = [], onActionComplete }) => {
    const { t } = useTranslation();
    const [showRoleModal, setShowRoleModal] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);
    const [userRoles, setUserRoles] = useState([]);
    const [availableRoles] = useState(['Admin', 'User']);

    const [blockUser, { isLoading: isBlocking }] = useBlockUserMutation();
    const [unblockUser, { isLoading: isUnblocking }] = useUnblockUserMutation();
    const [deleteUser, { isLoading: isDeleting }] = useDeleteUserMutation();
    const [setUserRole, { isLoading: isSettingRole }] = useSetUserRoleMutation();

    const handleBlock = async (userId) => {
        try {
            await blockUser(userId).unwrap();
            toast.success(t('admin.users.blockSuccess', 'User blocked successfully.'));
            if (onActionComplete) onActionComplete();
        } catch (err) {
            toast.error(err?.data?.message || t('admin.users.blockError', 'Failed to block user.'));
        }
    };

    const handleUnblock = async (userId) => {
        try {
            await unblockUser(userId).unwrap();
            toast.success(t('admin.users.unblockSuccess', 'User unblocked successfully.'));
            if (onActionComplete) onActionComplete();
        } catch (err) {
            toast.error(err?.data?.message || t('admin.users.unblockError', 'Failed to unblock user.'));
        }
    };

    const handleDelete = async (userId, username) => {
        if (window.confirm(t('admin.users.deleteConfirm', 'Are you sure you want to delete user {{username}}? This action cannot be undone.', { username }))) {
            try {
                await deleteUser(userId).unwrap();
                toast.success(t('admin.users.deleteSuccess', 'User deleted successfully.'));
                if (onActionComplete) onActionComplete();
            } catch (err) {
                toast.error(err?.data?.message || t('admin.users.deleteError', 'Failed to delete user.'));
            }
        }
    };

    const handleShowRoleModal = (user) => {
        setSelectedUser(user);
        setUserRoles(user.roles || []);
        setShowRoleModal(true);
    };

    const handleCloseRoleModal = () => {
        setShowRoleModal(false);
        setSelectedUser(null);
        setUserRoles([]);
    };

    const handleRoleChange = (roleName) => {
        setUserRoles(prevRoles =>
            prevRoles.includes(roleName)
                ? prevRoles.filter(r => r !== roleName)
                : [...prevRoles, roleName]
        );
    };

    const handleSaveRoles = async () => {
        if (!selectedUser) return;

        const initialRoles = selectedUser.roles || [];
        const rolesToAdd = userRoles.filter(r => !initialRoles.includes(r));
        const rolesToRemove = initialRoles.filter(r => !userRoles.includes(r));

        const promises = [];
        rolesToAdd.forEach(roleName => {
            promises.push(setUserRole({ userId: selectedUser.id, roleName, addRole: true }).unwrap());
        });
        rolesToRemove.forEach(roleName => {
            promises.push(setUserRole({ userId: selectedUser.id, roleName, addRole: false }).unwrap());
        });

        try {
            await Promise.all(promises);
            toast.success(t('admin.users.setRoleSuccess', 'User roles updated successfully.'));
            handleCloseRoleModal();
            if (onActionComplete) onActionComplete();
        } catch (err) {
            toast.error(err?.data?.message || t('admin.users.setRoleError', 'Failed to update user roles.'));
        }
    };

    if (!users || users.length === 0) {
        return <p className="text-muted">{t('admin.users.noUsers', 'No users found.')}</p>;
    }

    const isLoadingAction = isBlocking || isUnblocking || isDeleting || isSettingRole;

    return (
        <>
            <Table striped bordered hover responsive size="sm" className="align-middle">
                <thead>
                    <tr>
                        <th>{t('admin.users.colUsername', 'Username')}</th>
                        <th>{t('admin.users.colEmail', 'Email')}</th>
                        <th>{t('admin.users.colRoles', 'Roles')}</th>
                        <th>{t('admin.users.colStatus', 'Status')}</th>
                        <th className="text-center">{t('admin.users.colActions', 'Actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.id}>
                            <td>{user.userName}</td>
                            <td>{user.email}</td>
                            <td>
                                {user.roles?.map(role => (
                                    <Badge key={role} pill bg={role === 'Admin' ? 'danger' : 'secondary'} className="me-1">
                                        {role}
                                    </Badge>
                                ))}
                                {(!user.roles || user.roles.length === 0) && <small className='text-muted'>N/A</small>}
                            </td>
                            <td>
                                <Badge bg={user.isBlocked ? 'warning' : 'success'} text={user.isBlocked ? 'dark' : ''}>
                                    {user.isBlocked
                                        ? <><BsPersonFillLock className="me-1" /> {t('admin.users.statusBlocked', 'Blocked')}</>
                                        : <><BsPersonFillCheck className="me-1" /> {t('admin.users.statusActive', 'Active')}</>
                                    }
                                </Badge>
                            </td>
                            <td className="text-center">
                                <Dropdown>
                                    <Dropdown.Toggle variant="link" size="sm" id={`dropdown-actions-${user.id}`} className="text-decoration-none p-0" disabled={isLoadingAction}>
                                        <BsThreeDotsVertical />
                                    </Dropdown.Toggle>

                                    <Dropdown.Menu align="end">
                                        {user.isBlocked ? (
                                            <Dropdown.Item onClick={() => handleUnblock(user.id)} disabled={isUnblocking}>
                                                <BsPersonFillCheck className="me-2" /> {t('admin.users.actionUnblock', 'Unblock')}
                                            </Dropdown.Item>
                                        ) : (
                                            <Dropdown.Item onClick={() => handleBlock(user.id)} disabled={isBlocking}>
                                                <BsPersonFillLock className="me-2" /> {t('admin.users.actionBlock', 'Block')}
                                            </Dropdown.Item>
                                        )}
                                        <Dropdown.Item onClick={() => handleShowRoleModal(user)}>
                                            <BsPersonFillAdd className="me-2" /> {t('admin.users.actionSetRole', 'Edit Roles')}
                                        </Dropdown.Item>
                                        <Dropdown.Divider />
                                        <Dropdown.Item onClick={() => handleDelete(user.id, user.userName)} disabled={isDeleting} className="text-danger">
                                            <BsTrash className="me-2" /> {t('common.delete', 'Delete')}
                                        </Dropdown.Item>
                                    </Dropdown.Menu>
                                </Dropdown>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>

            {selectedUser && (
                <Modal show={showRoleModal} onHide={handleCloseRoleModal} centered>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Roles for {selectedUser.userName}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <p>Select the roles for this user:</p>
                        <Form>
                            {availableRoles.map(roleName => (
                                <Form.Check
                                    key={roleName}
                                    type="checkbox"
                                    id={`role-${roleName}-${selectedUser.id}`}
                                    label={roleName}
                                    checked={userRoles.includes(roleName)}
                                    onChange={() => handleRoleChange(roleName)}
                                />
                            ))}
                        </Form>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCloseRoleModal}>
                            {t('common.cancel', 'Cancel')}
                        </Button>
                        <Button variant="primary" onClick={handleSaveRoles} disabled={isSettingRole}>
                            {isSettingRole ? <Spinner animation="border" size="sm" /> : t('common.save', 'Save')} Roles
                        </Button>
                    </Modal.Footer>
                </Modal>
            )}
        </>
    );
};

export default UserManagementTable;