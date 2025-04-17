"use client"

import HttpServices from "../../../lib/HttpServices";
import {useAuth} from "../../AuthContext";
import React, {useEffect, useState} from "react";
import IngredientDto, {ingredientHeaders, ingredientHeadersStaff} from "../../../lib/models/ingredient/IngredientDto";
import IngredientQueryDto from "../../../lib/models/ingredient/IngredientQueryDto";
import TypeDto from "../../../lib/models/type/TypeDto";
import UnitDto from "../../../lib/models/unit/UnitDto";
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
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faFolderPlus} from "@fortawesome/free-solid-svg-icons/faFolderPlus";
import {faTrash} from "@fortawesome/free-solid-svg-icons/faTrash";
import {faPenToSquare} from "@fortawesome/free-solid-svg-icons/faPenToSquare";
import Modals from "../../../components/CustomModal";
import {toast} from "react-toastify";
import {isEmpty} from "@nextui-org/shared-utils";

export default function IngredientComponent() {
    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const [ingredient, setIngredient] = useState<BasicDto<IngredientDto>>({
        error    : "",
        isSuccess: false,
        value    : {
            amount   : 0,
            resultDto: [{
                id      : 0,
                name    : "",
                quantity: 0,
                type    : {
                    id  : 0,
                    name: ""
                },
                unit    : {
                    id  : 0,
                    name: ""
                }
            }]
        }
    });
    const [editObj, setEditObj] = useState<IngredientDto>({
        id      : 0,
        name    : "",
        quantity: 0,
        type    : {
            id  : 0,
            name: ""
        },
        unit    : {
            id  : 0,
            name: ""
        }
    });
    const [newIngredient, setNewIngredient] = useState<IngredientQueryDto>({
        id      : null,
        name    : null,
        quantity: null,
        type_Id : null,
        unit_Id : null
    });
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();
    const [editModal, setEditModal] = useState(true);
    const [types, setTypes] = useState<TypeDto[]>([]);
    const [units, setUnits] = useState<UnitDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isInvalid, setIsInvalid] = useState(false);
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentIdForDeletion, setCurrentIdForDeletion] = useState<number | null>(null);

    const foodItemAPI: string = "/DataManagement/food-item";
    const typeAPI: string = "/DataManagement/type";
    const unitAPI: string = "/DataManagement/unit";

    const showToast = (message: string) => {
        toast(message);
    }

    const retrieveIngredient = async (ingredientQueryDto: IngredientQueryDto) => {
        try {
            const server_res = await (await httpServices.callAPI(`${foodItemAPI}/read`, ingredientQueryDto, "POST", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateIngredient = async (ingredientQueryDto: IngredientQueryDto) => {
        try {
            const server_res = await (await httpServices.callAPI(`${foodItemAPI}/update`, ingredientQueryDto, "POST", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const deleteIngredient = async (id: number) => {
        try {
            const server_res = await (await httpServices.callAPI(`${foodItemAPI}/${id}`, null, "DELETE", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const createIngredient = async (ingredientQueryDto: IngredientQueryDto) => {
        try {
            const server_res = await (await httpServices.callAPI(`${foodItemAPI}/creation`, ingredientQueryDto, "POST", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const retrieveType = async () => {
        try {
            const server_res = await (await httpServices.callAPI(`${typeAPI}/read`, {
                id  : null,
                name: null
            }, "POST", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const retrieveUnit = async () => {
        try {
            const server_res = await (await httpServices.callAPI(`${unitAPI}/read`, {
                id  : null,
                name: null
            }, "POST", token)).json();
            return server_res as BasicDto<IngredientDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const handleCreate = () => {
        setEditModal(false);
        onOpen();
    }

    const handleDelete = (id: number) => {
        setCurrentIdForDeletion(id);
        setDeletionModalOpen(true);
    }

    const handleEdit = (id: number) => {
        (async () => {
            setEditModal(true);
            const server_res = await retrieveIngredient({
                id      : id,
                type_Id : null,
                quantity: null,
                unit_Id : null,
                name    : null
            });

            if (!server_res) {
                throw new Error("Failed to retrieve ingredient for delete");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }
            const [ingredientDto] = server_res.value.resultDto;
            setEditObj(ingredientDto);
            onOpen();
        })().catch(error => {
            if (error instanceof Error) {
                toast.error(error.message);
            } else {
                toast.error("Service crashed.");
            }
        });
    }

    const confirmEdition = () => {
        (async () => {

            if (!editObj.quantity || editObj.quantity < 0) {
                setIsInvalid(true);
                return;
            }

            let server_res = await updateIngredient({
                id      : editObj.id,
                name    : editObj.name,
                quantity: editObj.quantity,
                type_Id : editObj.type.id,
                unit_Id : editObj.unit.id
            });

            if (!server_res) {
                throw new Error("Failed to update ingredient.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }
            let retrieveIngredientRes = await retrieveIngredient({
                id      : null,
                name    : null,
                quantity: null,
                unit_Id : null,
                type_Id : null
            });

            if (!retrieveIngredientRes) {
                throw new Error("Failed to retrieve updated ingredient");
            }

            if (!retrieveIngredientRes.isSuccess) {
                throw new Error(`Fail - ${retrieveIngredientRes.error}`);
            }
            setIngredient(retrieveIngredientRes!);
            onClose();
            setIsInvalid(false);
        })().catch(error => {
            if (error instanceof Error) {
                toast.error(error.message);
            } else {
                toast.error("Service crashed.");
            }
        });
    }

    const confirmDelete = () => {
        (async () => {
            if (!currentIdForDeletion){
                throw new Error("The current id for deletion is null.")
            }
            
            const server_res = await deleteIngredient(currentIdForDeletion);

            if (!server_res) {
                throw new Error("Failed to delete ingredient.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const retrieveUpdatedIngredient = await retrieveIngredient({
                id      : null,
                name    : null,
                type_Id : null,
                unit_Id : null,
                quantity: null
            });

            if (!retrieveUpdatedIngredient) {
                throw new Error("Failed to retrieve ingredient after delete.");
            }

            if (!retrieveUpdatedIngredient.isSuccess) {
                throw new Error(`Fail - ${retrieveUpdatedIngredient.error}`);
            }

            setIngredient(retrieveUpdatedIngredient!);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed.");
            }
        }).finally(() => setDeletionModalOpen(false));
    }
    
    const confirmCreation = () => {
        (async () => {
            if (!newIngredient.quantity || newIngredient.quantity < 0) {
                setIsInvalid(true);
                return;
            }
            const {
                name,
                type_Id,
                unit_Id,
                quantity
            } = newIngredient;
            let server_res = await createIngredient({
                id      : null,
                name    : name,
                type_Id : type_Id,
                unit_Id : unit_Id,
                quantity: quantity
            });

            if (!server_res) {
                throw new Error("Failed to create ingredient.");
            }

            if (!server_res!.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            let retrieveIngredientRes = await retrieveIngredient({
                id      : null,
                name    : null,
                quantity: null,
                unit_Id : null,
                type_Id : null
            });

            if (!retrieveIngredientRes) {
                throw new Error("Failed to retrieve created ingredient.");
            }

            if (!retrieveIngredientRes.isSuccess) {
                throw new Error(`Fail - ${retrieveIngredientRes.error}`);
            }
            setIngredient(retrieveIngredientRes!);
            onClose();
            setIsInvalid(false);
        })().catch(error => {
            if (error instanceof Error) {
                toast.error(error.message);
            } else {
                toast.error("Service crashed.");
            }
        });
    }

    const cancelEdition = () => {
        setIsInvalid(false);
        onClose();
    }

    const cancelCreation = () => {
        setIsInvalid(false);
        onClose();
    }

    const updateName = (name: string) => {
        const {
            id,
            quantity,
            unit,
            type
        } = editObj;

        setEditObj({
            id      : id,
            name    : name,
            unit    : unit,
            type    : type,
            quantity: quantity
        });
    }

    const updateQuantity = (quantityStr: string) => {

        const quantity = parseInt(quantityStr);

        if ((isNaN(quantity) || !Number.isInteger(quantity) || quantity.toString() !== quantityStr) && !isEmpty(quantityStr)) {
            setIsInvalid(true);
            return;
        }

        if (quantity < 0) {
            setIsInvalid(true);
            return;
        }

        const {
            id,
            name,
            unit,
            type
        } = editObj;

        setEditObj({
            id      : id,
            name    : name,
            unit    : unit,
            type    : type,
            quantity: quantity
        });
        setIsInvalid(false);
    }

    const updateUnit = (unit: string[]) => {
        const {
            id,
            name,
            quantity,
            type
        } = editObj;
        const newUnitId: number = parseInt(unit[0]);
        const [newUnit] = units.filter(iunit => iunit.id === newUnitId);
        setEditObj({
            id      : id,
            name    : name,
            unit    : newUnit,
            type    : type,
            quantity: quantity
        });
    }

    const updateType = (type: string[]) => {
        const {
            id,
            name,
            quantity,
            unit
        } = editObj;
        const newTypeId: number = parseInt(type[0]);
        const [newType] = types.filter(itype => itype.id === newTypeId);
        setEditObj({
            id      : id,
            name    : name,
            quantity: quantity,
            unit    : unit,
            type    : newType
        });
    }

    const createNewName = (name: string) => {
        const {
            id,
            type_Id,
            unit_Id,
            quantity
        } = newIngredient;
        setNewIngredient({
            id      : id,
            type_Id : type_Id,
            unit_Id : unit_Id,
            quantity: quantity,
            name    : name
        });
    }

    const createNewQuantity = (quantity: number) => {
        const {
            id,
            type_Id,
            unit_Id,
            name
        } = newIngredient;
        setNewIngredient({
            id      : id,
            type_Id : type_Id,
            unit_Id : unit_Id,
            quantity: quantity,
            name    : name
        });
    }

    const createNewUnit = (unit: string[]) => {
        const {
            id,
            quantity,
            type_Id,
            name
        } = newIngredient;
        setNewIngredient({
            id      : id,
            type_Id : type_Id,
            unit_Id : parseInt(unit[0]),
            quantity: quantity,
            name    : name
        });
    }

    const createNewType = (type: string[]) => {
        const {
            id,
            quantity,
            unit_Id,
            name
        } = newIngredient;
        setNewIngredient({
            id      : id,
            type_Id : parseInt(type[0]),
            unit_Id : unit_Id,
            quantity: quantity,
            name    : name
        });
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
                        value={editObj.name.toString()}
                        onChange={(event) => updateName(event.target.value)}
                    />
                    <Input
                        label={"Quantity"}
                        type={"number"}
                        min={0}
                        isInvalid={isInvalid}
                        defaultValue={editObj.quantity.toString()}
                        value={editObj.quantity.toString()}
                        isRequired={true}
                        onChange={(event) => updateQuantity(event.target.value)}
                    />
                    <Select
                        label={"Units"}
                        selectionMode={"single"}
                        selectedKeys={editObj.unit.id.toString()}
                        isRequired={true}
                        onSelectionChange={(selection) => updateUnit(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            units.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.id}
                                >
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                    <Select
                        label={"Type"}
                        selectionMode={"single"}
                        selectedKeys={editObj.type.id.toString()}
                        isRequired={true}
                        onSelectionChange={(selection) => updateType(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            types.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.id}
                                >
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
                    <Input
                        label={"Name"}
                        type={"text"}
                        isRequired={true}
                        onChange={(event) => createNewName(event.target.value)}
                    />
                    <Input
                        label={"Quantity"}
                        type={"number"}
                        min={0}
                        isInvalid={isInvalid}
                        isRequired={true}
                        onChange={(event) => createNewQuantity(parseInt(event.target.value))}
                    />
                    <Select
                        label={"Units"}
                        selectionMode={"single"}
                        isRequired={true}
                        onSelectionChange={(selection) => createNewUnit(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            units.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.name}
                                >
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                    <Select
                        label={"Type"}
                        selectionMode={"single"}
                        isRequired={true}
                        onSelectionChange={(selection) => createNewType(Array.from(selection, opt => opt.toString()))}
                    >
                        {
                            types.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.name}
                                >
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                </>
            )
        }
    }

    const generateOptionalFields = () => {
        if (user?.role.includes("Manager")) {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        ingredient.value.resultDto.map((dto) => {
                            return (
                                <TableRow key={dto.id}>
                                    <TableCell>{dto.id}</TableCell>
                                    <TableCell>{dto.name}</TableCell>
                                    <TableCell>{dto.quantity}</TableCell>
                                    <TableCell>{dto.unit.name}</TableCell>
                                    <TableCell>{dto.type.name}</TableCell>
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
                        })
                    }
                </TableBody>
            );
        } else {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        ingredient.value.resultDto.map((dto) => {
                            return (
                                <TableRow key={dto.id}>
                                    <TableCell>{dto.id}</TableCell>
                                    <TableCell>{dto.name}</TableCell>
                                    <TableCell>{dto.quantity}</TableCell>
                                    <TableCell>{dto.unit.name}</TableCell>
                                    <TableCell>{dto.type.name}</TableCell>
                                </TableRow>
                            )
                        })
                    }
                </TableBody>
            );
        }
    }

    useEffect(() => {
        (async () => {
            const server_res = await retrieveIngredient({
                id      : null,
                name    : null,
                type_Id : null,
                unit_Id : null,
                quantity: null
            });

            if (!server_res) {
                throw new Error("Failed to retrieve ingredient");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            setIngredient(server_res!);
            let typeRetrieve = await retrieveType();

            if (!typeRetrieve) {
                throw new Error("Failed to retrieve type");
            }

            if (!typeRetrieve.isSuccess) {
                throw new Error(`Fail - ${typeRetrieve.error}`);
            }

            setTypes(typeRetrieve!.value.resultDto);

            const unitRetrieve = await retrieveUnit();

            if (!unitRetrieve) {
                throw new Error("Failed to retrieve unit");
            }

            if (!unitRetrieve.isSuccess) {
                throw new Error(`Fail - ${unitRetrieve.error}`);
            }

            setUnits(unitRetrieve!.value.resultDto);
            setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                toast.error(error.message);
            } else {
                toast.error("Service crashed.");
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
                aria-label={"Food Item"}
                topContent={<h1 className={"w-full text-center"}>Food Management</h1>}
            >
                    <TableHeader>
                        {
                            user?.role.includes("Manager")
                            ?
                            ingredientHeaders.map(tableHeader =>
                                <TableColumn
                                    key={tableHeader.key}
                                >{tableHeader.label}
                                </TableColumn>
                            )
                            : ingredientHeadersStaff.map(tableHeader =>
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
        </>
    );
}