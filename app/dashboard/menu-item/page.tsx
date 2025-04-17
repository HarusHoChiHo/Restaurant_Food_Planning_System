"use client"
import React, {Fragment, useEffect, useState} from "react";
import MenuItemDto, {menuItemHeaders, menuItemHeadersStaff} from "../../../lib/models/menu/MenuItemDto";
import {
    Button,
    Input,
    Spinner,
    Table,
    TableBody,
    TableCell,
    TableColumn,
    TableHeader,
    TableRow,
    useDisclosure
} from "@nextui-org/react";
import HttpServices from "../../../lib/HttpServices";
import {useAuth} from "../../AuthContext";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faPenToSquare} from "@fortawesome/free-solid-svg-icons/faPenToSquare";
import {faTrash} from "@fortawesome/free-solid-svg-icons/faTrash";
import {faFolderPlus} from "@fortawesome/free-solid-svg-icons/faFolderPlus";
import Modals from "../../../components/CustomModal";
import {toast} from "react-toastify";

export default function MenuItemComponent() {
    const [menuItem, setMenuItem] = useState<BasicDto<MenuItemDto>>(
        {
            error    : "",
            isSuccess: false,
            value    : {
                amount   : 0,
                resultDto: [{
                    id  : 0,
                    name: ""
                }]
            }
        });


    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const [editObj, setEditObj] = useState<MenuItemDto>(
        {
            id  : 0,
            name: ""
        }
    );
    const [editModal, setEditModal] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const [newName, setNewName] = useState("");
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentIdForDeletion, setCurrentIdForDeletion] = useState<number | null>(null);
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();

    const menuItemAPI: string = "/DataManagement/menu-item";

    useEffect(() => {
        (async () => {
            const server_res = await retrieveMenuItem({
                id  : null,
                name: null
            });

            if (!server_res) {
                throw new Error("Failed to retrieve menu item.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            setMenuItem(server_res);

            setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }, []);

    const showToast = (message: string) => {
        toast(message);
    }

    const createMenuItem = async function () {
        try {
            const response = await (await httpServices.callAPI(`${menuItemAPI}/creation`, {
                id  : null,
                name: newName
            }, "POST", token)).json();
            return response as BasicDto<MenuItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }


    const retrieveMenuItem = async function (menuItemQuery: MenuItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemAPI}/read`, menuItemQuery, "POST", token)).json();
            return response as BasicDto<MenuItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateMenuItem = async function (menuItemQuery: MenuItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemAPI}/update`, menuItemQuery, "POST", token)).json();
            return response as BasicDto<MenuItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const deleteMenuItem = async function (id: number) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemAPI}/${id}`, {}, "DELETE", token)).json();
            return response as BasicDto<MenuItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const handleEdit = (id: number) => {
        setEditModal(true);
        (async () => {
            const server_res = await retrieveMenuItem({
                id  : id,
                name: null
            });
            if (!server_res) {
                throw new Error("Failed to retrieve menu item.");
            }
            if (!server_res.isSuccess) {
                throw new Error("failed to retrieve menu with id");
            }

            const existingMenuItem: MenuItemDto | undefined = (server_res.value.resultDto as MenuItemDto[]).pop();

            if (!existingMenuItem) {
                throw new Error("Existing menu is null.");
            }
            setEditObj(existingMenuItem);
            onOpen();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const handleDelete = (id: number) => {
        setCurrentIdForDeletion(id);
        setDeletionModalOpen(true);
    }

    const handleCreate = () => {
        setEditModal(false);
        onOpen();
    }

    const insertName = (name: string) => {
        const {id} = editObj;
        setEditObj({
            id  : id,
            name: name
        });
    }

    const createName = (name: string) => {
        setNewName(name);
    }

    const cancelEdition = () => {
        onClose();
    }

    const confirmDelete = () => {
        (async () => {
            if (!currentIdForDeletion) {
                throw new Error("The current id for deletion is null");
            }

            const server_res = await deleteMenuItem(currentIdForDeletion);
            if (!server_res) {
                throw new Error("Failed to delete menu item.");
            }
            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }
            const updatedList = await retrieveMenuItem({
                id  : null,
                name: null
            });

            if (!updatedList) {
                throw new Error("Failed to retrieve menu item.");
            }

            if (!updatedList.isSuccess) {
                throw new Error(`Fail - ${updatedList.error}`);
            }

            setMenuItem(updatedList);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }).finally(() => {
            setDeletionModalOpen(false);
            setCurrentIdForDeletion(null);
        });
    }
    
    const confirmEdition = () => {
        const {id, name} = editObj;

        (async () => {
            const server_res = await updateMenuItem({
                id  : id,
                name: name
            });

            if (!server_res) {
                throw new Error("Update menu failed");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const menuReadResponse = await retrieveMenuItem({
                id  : null,
                name: null
            });

            if (!menuReadResponse) {
                throw new Error("Retrieve updated menu failed");
            }

            if (!menuReadResponse.isSuccess) {
                throw new Error(`Fail - ${menuReadResponse.error}`);
            }
            setMenuItem(menuReadResponse);
            onClose();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const cancelCreation = () => {
        onClose();
    }

    const confirmCreation = () => {
        (async () => {
            const server_res = await createMenuItem();

            if (!server_res) {
                throw new Error("Failed to create menu item.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const updatedMenuItemResponse = await retrieveMenuItem({
                id  : null,
                name: null
            });

            if (!updatedMenuItemResponse) {
                throw new Error("Failed to retrieved created menu item.");
            }

            if (!updatedMenuItemResponse.isSuccess) {
                throw new Error("Failed to create new menu item");
            }

            setMenuItem(updatedMenuItemResponse);
            onClose();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const renderContent = () => {
        if (editModal) {
            return (
                <>
                    <Input
                        label={"Name"}
                        placeholder={editObj.name}
                        defaultValue={editObj.name}
                        onValueChange={(name) => insertName(name)}
                    />
                </>
            )
        } else {
            return (
                <Input
                    label={"Name"}
                    placeholder={"Insert a name"}
                    onInput={(event) => createName(event.currentTarget.value)}
                    onValueChange={(value) => createName(value)}
                    onChange={(value) => createName(value.target.value)}
                />
            )
        }
    }

    const generateOptionalFields = () => {
        if (user?.role.includes("Manager")) {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        menuItem.value.resultDto.map((dto) =>
                            <TableRow key={dto.id}>
                                <TableCell>{dto.id}</TableCell>
                                <TableCell>{dto.name}</TableCell>
                                <TableCell width={"30px"}>
                                    <Button
                                        size={"sm"}
                                        onClick={() => handleDelete(dto.id)}
                                    >
                                        <FontAwesomeIcon
                                            icon={faTrash}
                                            id={dto.id.toString()}
                                        />
                                    </Button>
                                </TableCell>
                                <TableCell width={"30px"}>
                                    <Button
                                        size={"sm"}
                                        onClick={() => handleEdit(dto.id)}
                                    >
                                        <FontAwesomeIcon icon={faPenToSquare} />
                                    </Button>
                                </TableCell>
                            </TableRow>
                        )
                    }
                </TableBody>
            );
        } else {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        menuItem.value.resultDto.map((dto) =>
                            <TableRow key={dto.id}>
                                <TableCell>{dto.id}</TableCell>
                                <TableCell>{dto.name}</TableCell>
                            </TableRow>
                        )
                    }
                </TableBody>
            );
        }
    }

    if (isLoading) {
        return <Spinner />;
    }

    return (
        <>
            {
                user?.role.includes("Manager")
                ? (<div className={"w-full flex flex-row justify-end p-2"}>
                    <Button
                        variant={"solid"}
                        startContent={<FontAwesomeIcon icon={faFolderPlus} />}
                        onClick={handleCreate}
                        className={"w-3/12"}
                        color={"success"}
                    />
                </div>)
                : (<></>)
            }
            <Table
                aria-label={"Course"}
                topContent={<h1 className={"w-full text-center"}>Course Management</h1>}
            >
                <TableHeader>
                    {
                        user?.role.includes("Manager")
                        ?
                        menuItemHeaders.map(tableHeader =>
                            <TableColumn
                                key={tableHeader.key}
                            >{tableHeader.label}
                            </TableColumn>
                        )
                        : menuItemHeadersStaff.map(tableHeader =>
                            <TableColumn
                                key={tableHeader.key}
                            >{tableHeader.label}
                            </TableColumn>
                        )
                    }
                </TableHeader>
                {
                    generateOptionalFields()
                }
            </Table>
            <Modals
                isOpen={isOpen}
                onOpenChange={onOpenChange}
                onCancel={() => editModal
                                ? cancelEdition()
                                : cancelCreation()}
                onConfirm={() => editModal
                                 ? confirmEdition()
                                 : confirmCreation()}
                header={editModal
                        ? "Edit"
                        : "Create"}
                hideCloseButton={false}
            >
                {
                    renderContent()
                }
            </Modals>
            <Modals
                isOpen={isDeletionModalOpen}
                onOpenChange={setDeletionModalOpen}
                onCancel={() => setDeletionModalOpen(false)}
                onConfirm={confirmDelete}
                header={"Confirm Deletion"}
                hideCloseButton={false}
            >
                <p>Are you sure you want to delete it?</p>
            </Modals>
            <Modals
                isOpen={isDeletionModalOpen}
                onOpenChange={setDeletionModalOpen}
                onCancel={() => setDeletionModalOpen(false)}
                onConfirm={confirmDelete}
                header={"Confirm Deletion"}
                hideCloseButton={false}
            >
                <p>Are you sure you want to delete it?</p>
            </Modals>
        </>
    );
}