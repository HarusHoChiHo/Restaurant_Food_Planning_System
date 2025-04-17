"use client"

import HttpServices from "../../../lib/HttpServices";
import {useAuth} from "../../AuthContext";
import React, {Fragment, useEffect, useState} from "react";
import UnitDto from "../../../lib/models/unit/UnitDto";
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
import TypeDto, {typeHeaders, typeHeadersStaff} from "../../../lib/models/type/TypeDto";
import TypeQueryDto from "../../../lib/models/type/TypeQueryDto";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFolderPlus} from "@fortawesome/free-solid-svg-icons/faFolderPlus";
import {faTrash} from "@fortawesome/free-solid-svg-icons/faTrash";
import {faPenToSquare} from "@fortawesome/free-solid-svg-icons/faPenToSquare";
import Modals from "../../../components/CustomModal";
import {toast} from "react-toastify";

export default function TypeComponent() {
    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const typeAPI: string = "/DataManagement/type";
    const [type, setType] = useState<BasicDto<TypeDto>>({
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
    const [editObj, setEditObj] = useState<TypeDto>({
        id  : 0,
        name: ""
    });
    const [editModal, setEditModal] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();
    const [newName, setNewName] = useState<string>("");
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentIdForDeletion, setCurrentIdForDeletion] = useState<number | null>(null);

    const showToast = (message: string) => {
        toast(message);
    }

    const retrieveType = async (typeQueryDto: TypeQueryDto) => {
        try {
            const serverRes = await (await httpServices.callAPI(`${typeAPI}/read`, typeQueryDto, "POST", token)).json();
            return serverRes as BasicDto<UnitDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const createType = async (typeQueryDto: TypeQueryDto) => {
        try {
            const serverRes = await (await httpServices.callAPI(`${typeAPI}/creation`, typeQueryDto, "POST", token)).json();
            return serverRes as BasicDto<UnitDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const updateType = async (typeQueryDto: TypeQueryDto) => {
        try {
            const serverRes = await (await httpServices.callAPI(`${typeAPI}/update`, typeQueryDto, "POST", token)).json();
            return serverRes as BasicDto<UnitDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const deleteType = async (id: number) => {
        try {
            const serverRes = await (await httpServices.callAPI(`${typeAPI}/${id}`, null, "DELETE", token)).json();
            return serverRes as BasicDto<UnitDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const handleDelete = (id: number) => {
        setCurrentIdForDeletion(id);
        setDeletionModalOpen(true);
    }

    const handleEdit = (id: number) => {
        setEditModal(true);
        (async () => {
            const retrieveRes = await retrieveType({
                id  : id,
                name: null
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve updated type");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error(`Fail - ${retrieveRes.error}`);
            }

            setEditObj(retrieveRes.value.resultDto[0]);
            onOpen();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const handleCreate = () => {
        setEditModal(false);
        onOpen();
    }

    const updateName = (name: string) => {
        const {id} = editObj;
        setEditObj({
            id  : id,
            name: name
        });
    }

    const createNewName = (name: string) => {
        setNewName(name);
    }

    const confirmDelete = () => {
        (async () => {
            if (!currentIdForDeletion) {
                throw new Error("The current id for deletion is null");
            }

            const deleteRes = await deleteType(currentIdForDeletion);

            if (!deleteRes) {
                throw new Error("Failed to delete type.");
            }

            if (!deleteRes.isSuccess) {
                throw new Error(`Fail - ${deleteRes.error}`);
            }
            const retrieveRes = await retrieveType({
                id  : null,
                name: null
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve type after deletion.");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error(`Fail - ${retrieveRes.error}`);
            }

            setType(retrieveRes);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }).finally(() => {
            setDeletionModalOpen(false);
            setCurrentIdForDeletion(null);
        });
    }

    const confirmEdition = () => {
        (async () => {
            const updateRes = await updateType({
                id  : editObj.id,
                name: editObj.name
            });

            if (!updateRes) {
                throw new Error("Failed to update type");
            }

            if (!updateRes.isSuccess) {
                throw new Error(`Fail - ${updateRes.error}`);
            }

            const retrieveRes = await retrieveType({
                id  : null,
                name: null
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve updated type");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error(`Fail - ${retrieveRes.error}`);
            }
            setType(retrieveRes);
            closeModal();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const confirmCreation = () => {
        (async () => {
            const creationRes = await createType({
                id  : null,
                name: newName
            });

            if (!creationRes) {
                throw new Error("Failed to create type");
            }

            if (!creationRes.isSuccess) {
                throw new Error(`Fail - ${creationRes.error}`);
            }

            const retrieveRes = await retrieveType({
                id  : null,
                name: null
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve type");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error("Failed to retrieve newly created type");
            }
            setType(retrieveRes);
            closeModal();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const closeModal = () => {
        onClose();
    }

    const renderContent = () => {
        if (editModal) {
            return (
                <>
                    <Input
                        label={"Name"}
                        type={"text"}
                        defaultValue={editObj.name}
                        isRequired={true}
                        onChange={(event) => updateName(event.target.value)}
                    />
                </>
            )
        } else {
            return (
                <>
                    <Input
                        label={"Name"}
                        type={"text"}
                        isRequired={true}
                        onChange={(event) => createNewName(event.target.value)}
                    />
                </>
            )
        }
    }

    const generateOptionalFields = () => {
        if (user?.role.includes("Manager")) {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        type.value.resultDto.map((dto) =>
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
                        type.value.resultDto.map((dto) =>
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

    useEffect(() => {
        (async () => {
            const retrieveRes = await retrieveType({
                id  : null,
                name: null
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve type");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error(`Fail - ${retrieveRes.error}`);
            }
            setType(retrieveRes);
            setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }, []);

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
                        className={"w-3/12"}
                        onClick={handleCreate}
                        color={"success"}
                    />
                </div>)
                : (<></>)
            }
            <Table
                aria-label={"Type"}
                topContent={<h1 className={"w-full text-center"}>Type Management</h1>}
            >
                <TableHeader>
                    {
                        user?.role.includes("Manager")
                        ?
                        typeHeaders.map(tableHeader =>
                            <TableColumn
                                key={tableHeader.key}
                            >{tableHeader.label}
                            </TableColumn>
                        )
                        : typeHeadersStaff.map(tableHeader =>
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
                onCancel={closeModal}
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
        </>
    )
}