"use client"
import React, {Fragment, useEffect, useState} from "react";
import HttpServices from "../../../lib/HttpServices";
import UserDto, {userHeaders} from "../../../lib/models/user/UserDto";
import {useAuth} from "../../AuthContext";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faTrash} from "@fortawesome/free-solid-svg-icons/faTrash";
import {faPenToSquare} from "@fortawesome/free-solid-svg-icons/faPenToSquare";
import RoleDto from "../../../lib/models/RoleDto";
import {
    Button,
    Input,
    Select,
    SelectItem,
    Spinner,
    Table,
    TableBody,
    TableCell,
    TableColumn,
    TableHeader,
    TableRow,
    useDisclosure
} from "@nextui-org/react";
import Modals from "../../../components/CustomModal";
import {faFolderPlus} from "@fortawesome/free-solid-svg-icons/faFolderPlus";
import UserQueryDto from "../../../lib/models/user/UserQueryDto";
import {toast} from "react-toastify";
import {useRouter} from "next/navigation";

export default function Page() {
    const httpServices = new HttpServices();
    const {token,user} = useAuth();
    const router = useRouter();
    const [jsonObj, setJsonObj] = useState<BasicDto<UserDto>>({
        error    : "",
        isSuccess: false,
        value    : {
            amount   : 0,
            resultDto: [{
                id      : 0,
                password: "",
                userName: "",
                role    : []
            }]
        }
    });
    const [roles, setRoles] = useState<RoleDto[]>([]);
    const [editObj, setEditObj] = useState<UserDto>({
        id      : 0,
        password: "",
        role    : [],
        userName: ""
    });
    const [editModal, setEditModal] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentIdForDeletion, setCurrentIdForDeletion] = useState<number | null>(null);
    const [newData, setNewData] = useState({
        newName    : "",
        newPassword: "",
        newRoles   : [""]
    });
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();

    const userAPI = "/User";
    const roleAPI = "/Role";
    
    const showToast = (message: string) => {
        toast(message);
    }

    const handleDelete = (id: number) => {
        setCurrentIdForDeletion(id);
        setDeletionModalOpen(true);
    }

    const handleEdit = (id: number) => {
        setEditModal(true);
        let targetedObj;
        if (jsonObj.value.resultDto) {
            [targetedObj] = jsonObj.value.resultDto.filter((dto: {
                id: number;
            }) => dto.id === id);
            setEditObj(targetedObj);
        }
        onOpen();
    }

    const createUser = async (userQueryDto: UserQueryDto) => {
        try {
            const server_response = await (await httpServices.callAPI(`${userAPI}/register`, userQueryDto, "POST", token)).json();
            return server_response as BasicDto<UserDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const retrieveAllUser = async () => {
        try {
            const server_response = await (await httpServices.callAPI(`${userAPI}/`, null, "GET", token)).json();
            console.log(server_response);
            return server_response as BasicDto<UserDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const retrieveAllRoles = async () => {
        try {
            const server_response = await (await httpServices.callAPI(`${roleAPI}/read`, {
                name       : null,
                description: null,
                id         : null,
                createdDate: null
            }, "POST", token)).json();
            return server_response as BasicDto<RoleDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateUser = async () => {
        try {
            const server_response = await (await httpServices.callAPI("/user/update", editObj, "POST", token)).json();
            return server_response as BasicDto<UserDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateUserName = (updatedUserName: string) => {
        try {
            const {
                id,
                role,
                password
            } = editObj;
            setEditObj({
                id      : id,
                userName: updatedUserName,
                role    : role,
                password: password
            });
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updatePassword = (updatedPassword: string) => {
        try {
            const {
                id,
                role,
                userName
            } = editObj;

            setEditObj({
                id      : id,
                userName: userName,
                role    : role,
                password: updatedPassword
            });
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateRole = (updatedRole: string[]) => {
        try {
            const {
                id,
                userName,
                password
            } = editObj;
            setEditObj({
                id      : id,
                userName: userName,
                password: password,
                role    : updatedRole
            });
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const createNewName = (name: string) => {
        const {
            newPassword,
            newRoles
        } = newData;
        setNewData({
            newPassword: newPassword,
            newRoles   : newRoles,
            newName    : name
        });
    }

    const createNewPassword = (password: string) => {
        const {
            newName,
            newRoles
        } = newData;
        setNewData({
            newPassword: password,
            newRoles   : newRoles,
            newName    : newName
        });
    }

    const createNewRoles = (roles: string[]) => {
        const {
            newName,
            newPassword
        } = newData;
        setNewData({
            newPassword: newPassword,
            newRoles   : roles,
            newName    : newName
        });
    }

    const handleCreate = () => {
        setEditModal(false);
        onOpen();
    }

    const cancelEdition = () => {
        setEditObj({
            id      : 0,
            password: "",
            role    : [],
            userName: ""
        });
        onClose();
    }

    const confirmDelete = () => {
        (async () => {
            if(!currentIdForDeletion){
                throw new Error("The current id for deletion is null");
            }
            const server_res = await (await httpServices.callAPI(`/User/${currentIdForDeletion}`, null, "DELETE", token)).json() as BasicDto<UserDto>;

            if (!server_res) {
                throw new Error("Failed to delete list");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const updatedList = await (await httpServices.callAPI("/user", null, "GET", token)).json() as BasicDto<UserDto>;

            if (!updatedList) {
                throw new Error("Failed to retrieve updated list after deletion");
            }

            if (!updatedList.isSuccess) {
                throw new Error(`Fail - ${updatedList.error}`);

            }
            setJsonObj(updatedList);
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
        (async () => {
            const server_res = await updateUser();
            if (!server_res) {
                throw new Error("Failed to update user.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const retrieveUpdatedUserRes = await retrieveAllUser();

            if (!retrieveUpdatedUserRes) {
                throw new Error("Failed to retrieve all users.");
            }

            if (!retrieveUpdatedUserRes.isSuccess) {
                throw new Error(`Fail - ${retrieveUpdatedUserRes.error}`);
            }

            setJsonObj(retrieveUpdatedUserRes!);
            onClose();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const confirmCreation = () => {
        (async () => {
            const {
                newName,
                newPassword,
                newRoles
            } = newData;
            const server_response = await createUser({
                id      : null,
                userName: newName,
                password: newPassword,
                role    : newRoles
            });

            if (!server_response) {
                throw new Error("Failed to create user.");
            }

            if (!server_response.isSuccess) {
                throw new Error(`Fail - ${server_response.error}`);
            }

            const retrieveUpdatedUserResponse = await retrieveAllUser();

            if (!retrieveUpdatedUserResponse) {
                throw new Error("Failed to retrieve created user.");
            }

            if (!retrieveUpdatedUserResponse.isSuccess) {
                throw new Error(`Fail - ${retrieveUpdatedUserResponse.error}`);
            }

            setJsonObj(retrieveUpdatedUserResponse!);
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

    useEffect(() => {

        if (!user?.role.includes("Manager")){
            router.push("/dashboard");
            return;
        }
        
        (async () => {
            const server_res = await retrieveAllUser();

            if (!server_res) {
                throw new Error("Failed to retrieve all users");
            }

            if (!server_res.isSuccess || !server_res.value.resultDto) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            setJsonObj(server_res);

            const role_res = await retrieveAllRoles();

            if (!role_res) {
                throw new Error("Failed to retrieve all roles.");
            }

            if (!role_res.isSuccess) {
                throw new Error(`Fail - ${role_res.error}`);
            }

            setRoles(role_res.value.resultDto);
            setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }, []);
    
    const renderContent = () => {
        if (editModal) {
            return (
                <>
                    <Input label={"Username"}
                           type={"text"}
                           defaultValue={editObj.userName}
                           onChange={(event) => updateUserName(event.target.value)}
                    />

                    <Input label={"Password"}
                           type={"text"}
                           defaultValue={editObj.password}
                           onChange={(event) => updatePassword(event.target.value)}
                    />

                    <Select
                        label={"Roles"}
                        selectionMode={"multiple"}
                        selectedKeys={new Set(editObj.role)}
                        onSelectionChange={(selection) => updateRole(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            roles.map(value =>
                                <SelectItem key={value.name} value={value.name}>
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                </>
            )
        } else {
            return (
                <>
                    <Input label={"Username"}
                           type={"text"}
                           onChange={(event) => createNewName(event.target.value)}
                    />

                    <Input label={"Password"}
                           type={"text"}
                           onChange={(event) => createNewPassword(event.target.value)}
                    />

                    <Select
                        label={"Roles"}
                        selectionMode={"multiple"}
                        onSelectionChange={(selection) => createNewRoles(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            roles.map(value =>
                                <SelectItem key={value.name} value={value.name}>
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                </>
            )
        }
    }

    if (isLoading) {
        return <Spinner/>;
    }

    return (
        <>
            <div className={"w-full flex flex-row justify-end p-2"}>
                <Button variant={"solid"}
                        startContent={<FontAwesomeIcon icon={faFolderPlus}/>}
                        className={"w-3/12"}
                        onClick={handleCreate}
                        color={"success"}
                />
            </div>
            <Table
                aria-label={"User"}
                topContent={<h1 className={"w-full text-center"}>User Management</h1>}
            >
                <TableHeader>
                    {
                        userHeaders.map(tableHeader => <TableColumn
                            key={tableHeader.key}>{tableHeader.label}</TableColumn>)
                    }
                </TableHeader>
                <TableBody emptyContent={"No rows to display."}>
                    {
                        jsonObj.value.resultDto.map((userDto) =>
                            <TableRow key={userDto.id}>
                                <TableCell>{userDto.id}</TableCell>
                                <TableCell>{userDto.userName}</TableCell>
                                <TableCell>{userDto.password}</TableCell>
                                <TableCell>{userDto.role.join(", ")}</TableCell>
                                <TableCell  width={"30px"}>
                                    <Button size={"sm"}
                                            onClick={() => handleDelete(userDto.id)}
                                    >
                                        <FontAwesomeIcon icon={faTrash}
                                                         id={userDto.id.toString()}/>
                                    </Button>
                                </TableCell>
                                <TableCell  width={"30px"}>
                                    <Button size={"sm"}
                                            onClick={() => handleEdit(userDto.id)}
                                    >
                                        <FontAwesomeIcon icon={faPenToSquare}/>
                                    </Button>
                                </TableCell>
                            </TableRow>
                        )
                    }
                </TableBody>
            </Table>
            <Modals isOpen={isOpen}
                    onOpenChange={onOpenChange}
                    onCancel={() => editModal ? cancelEdition() : cancelCreation()}
                    onConfirm={() => editModal ? confirmEdition() : confirmCreation()}
                    header={editModal ? "Edit" : "Create"}
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
    );
}