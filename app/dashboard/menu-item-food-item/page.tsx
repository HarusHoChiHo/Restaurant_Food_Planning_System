"use client"

import React, {Fragment, useEffect, useState} from "react";
import MenuItemFoodItemDto, {
    mifiHeaders,
    mifiHeadersStaff
} from "../../../lib/models/menuitemfooditem/MenuItemFoodItemDto";
import HttpServices from "../../../lib/HttpServices";
import {useAuth} from "../../AuthContext";
import {toast} from "react-toastify";
import MenuItemFoodItemQueryDto from "../../../lib/models/menuitemfooditem/MenuItemFoodItemQueryDto";
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
import MenuItemDto from "../../../lib/models/menu/MenuItemDto";
import IngredientDto from "../../../lib/models/ingredient/IngredientDto";
import IngredientQueryDto from "../../../lib/models/ingredient/IngredientQueryDto";

export default function MenuItemFoodItemComponent() {
    const menuItemFoodItemAPI: string = "/DataManagement/menu-item-food-item";
    const menuItemAPI: string = "/DataManagement/menu-item";
    const foodItemAPI: string = "/DataManagement/food-item";
    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const [editObj, setEditObj] = useState<MenuItemFoodItemDto>(
        {
            foodItem   : {
                id      : 0,
                name    : "",
                quantity: 0,
                unit    : {
                    id  : 0,
                    name: ""
                },
                type    : {
                    id  : 0,
                    name: ""
                }
            },
            menuItem   : {
                id  : 0,
                name: ""
            },
            consumption: 0,
            foodItem_Id: 0,
            menuItem_Id: 0
        }
    );
    const [editModal, setEditModal] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const [invalidConsumption, setInvalidConsumption] = useState(false);
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentFoodIdForDeletion, setCurrentFoodIdForDeletion] = useState<number | null>(null);
    const [currentMenuItemIdForDeletion, setCurrentMenuItemIdForDeletion] = useState<number | null>(null);
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();
    const [menuItemFoodItem, setMenuItemFoodItem] = useState<BasicDto<MenuItemFoodItemDto>>({
        error    : "",
        isSuccess: false,
        value    : {
            amount   : 0,
            resultDto: [{
                consumption: 0,
                foodItem   : {
                    id      : 0,
                    name    : "",
                    quantity: 0,
                    unit    : {
                        id  : 0,
                        name: ""
                    },
                    type    : {
                        id  : 0,
                        name: ""
                    }
                },
                foodItem_Id: 0,
                menuItem   : {
                    id  : 0,
                    name: ""
                },
                menuItem_Id: 0
            }]
        }
    });
    const [menuItem, setMenuItem] = useState<BasicDto<MenuItemDto>>({
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
    const [foodItem, setFoodItem] = useState<BasicDto<IngredientDto>>({
        error    : "",
        isSuccess: false,
        value    : {
            amount   : 0,
            resultDto: [{
                id      : 0,
                name    : "",
                quantity: 0,
                unit    : {
                    id  : 0,
                    name: ""
                },
                type    : {
                    id  : 0,
                    name: ""
                }
            }]
        }
    });
    const [mifiQueryDto, setMifiQueryDto] = useState<MenuItemFoodItemQueryDto>({
        consumption: null,
        foodItem_Id: null,
        menuItem_Id: null
    });

    const createMenuItemFoodItem = async function (mifi: MenuItemFoodItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemFoodItemAPI}/creation`, mifi, "POST", token)).json();
            return response as BasicDto<MenuItemFoodItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const retrieveMenuItemFoodItem = async function (menuItemFoodItemQuery: MenuItemFoodItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemFoodItemAPI}/read`, menuItemFoodItemQuery, "POST", token)).json();
            return response as BasicDto<MenuItemFoodItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
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

    const updateMenuItemFoodItem = async function (menuItemFoodItemQuery: MenuItemFoodItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemFoodItemAPI}/update`, menuItemFoodItemQuery, "POST", token)).json();
            return response as BasicDto<MenuItemFoodItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const deleteMenuItemFoodItem = async function (menuItemFoodItemQueryDto: MenuItemFoodItemQueryDto) {
        try {
            const response = await (await httpServices.callAPI(`${menuItemFoodItemAPI}`, menuItemFoodItemQueryDto, "DELETE", token)).json();
            return response as BasicDto<MenuItemFoodItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const showToast = (message: string) => {
        toast(message);
    }

    const updateConsumption = (consumption: number) => {
        if (consumption < 1) {
            setInvalidConsumption(true);
            return;
        }

        const {
            foodItem_Id,
            menuItem_Id,
            menuItem,
            foodItem
        } = editObj;
        setEditObj({
            menuItem_Id: menuItem_Id,
            foodItem_Id: foodItem_Id,
            consumption: consumption,
            menuItem,
            foodItem
        });
        setInvalidConsumption(false);
    }

    const updateFoodItem = (fi: number) => {
       if (!fi){
          fi = 0;
       }
        const {
            menuItem_Id,
            consumption,
            menuItem,
            foodItem
        } = editObj;
        setEditObj({
            menuItem_Id: menuItem_Id,
            foodItem_Id: fi,
            consumption: consumption,
            menuItem,
            foodItem
        });
    }

    const createConsumption = (consumption: number) => {
        if (consumption < 1) {
            setInvalidConsumption(true);
            return;
        }
        const {
            foodItem_Id,
            menuItem_Id
        } = mifiQueryDto;
        setMifiQueryDto({
            menuItem_Id: menuItem_Id,
            foodItem_Id: foodItem_Id,
            consumption: consumption,
        });
        setInvalidConsumption(false);
    }

    const createMenuItem = (mi: number) => {
        const {
            foodItem_Id,
            consumption
        } = mifiQueryDto;
        setMifiQueryDto({
            menuItem_Id: mi,
            foodItem_Id: foodItem_Id,
            consumption: consumption
        });
    }

    const createFoodItem = (fi: number) => {
        const {
            menuItem_Id,
            consumption
        } = mifiQueryDto;
        setMifiQueryDto({
            menuItem_Id: menuItem_Id,
            foodItem_Id: fi,
            consumption: consumption
        });
    }

    const handleEdit = (menuItemId: number, foodItemId: number) => {
        setEditModal(true);
        (async () => {

            const [tempEditObj] = menuItemFoodItem.value.resultDto.filter(mifi => mifi.foodItem_Id === foodItemId && mifi.menuItem_Id === menuItemId);

            if (!tempEditObj) {
                throw new Error("Failed to find related item.");
            }
            setEditObj(tempEditObj);
            onOpen();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const handleDelete = (menuItemId: number, foodItemId: number) => {
        setCurrentMenuItemIdForDeletion(menuItemId);
        setCurrentFoodIdForDeletion(foodItemId);
        setDeletionModalOpen(true);
    }

    const handleCreate = () => {
        setEditModal(false);
        onOpen();
    }

    const cancelEdition = () => {
        onClose();
    }

    const confirmDelete = () => {
        (async () => {
            if (!currentMenuItemIdForDeletion || !currentFoodIdForDeletion) {
                throw new Error("The current id for deletion is null");
            }

            const server_res = await deleteMenuItemFoodItem({
                menuItem_Id: currentMenuItemIdForDeletion,
                foodItem_Id: currentFoodIdForDeletion,
                consumption: null
            });

            if (!server_res) {
                throw new Error("Failed to delete MenuItemFoodItem.");
            }

            if (!server_res.isSuccess) {
                throw new Error(`Fail - ${server_res.error}`);
            }

            const menuItemFoodItem_res = await retrieveMenuItemFoodItem({
                menuItem_Id: null,
                foodItem_Id: null,
                consumption: null
            });

            if (!menuItemFoodItem_res) {
                throw new Error("Failed to retrieve updated MenuItemFoodItem.");
            }

            if (!menuItemFoodItem_res.isSuccess) {
                throw new Error(`Fail - ${menuItemFoodItem_res.error}`);
            }

            setMenuItemFoodItem(menuItemFoodItem_res);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }).finally(() => {
            setDeletionModalOpen(false);
            setCurrentFoodIdForDeletion(null);
            setCurrentMenuItemIdForDeletion(null);
        });
    }
    
    const confirmEdition = () => {
        (async () => {
            const {
                foodItem_Id,
                menuItem_Id,
                consumption
            } = editObj;

            const update_res = await updateMenuItemFoodItem({
                menuItem_Id: menuItem_Id,
                foodItem_Id: foodItem_Id,
                consumption: consumption
            });

            if (!update_res) {
                throw new Error("Failed to update.");
            }

            if (!update_res.isSuccess) {
                throw new Error(`Fail - ${update_res.error}`);
            }

            const retrieve_res = await retrieveMenuItemFoodItem({
                menuItem_Id: null,
                foodItem_Id: null,
                consumption: null
            });

            if (!retrieve_res) {
                throw new Error("Failed to retrieve updated items.");
            }

            if (!retrieve_res.isSuccess) {
                throw new Error(`Fail - ${retrieve_res.error}`);
            }

            setMenuItemFoodItem(retrieve_res);
            onClose();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const cancelCreation = () => {
        setInvalidConsumption(false);
        onClose();
    }

    const confirmCreation = () => {
        (async () => {

            const creation_res = await createMenuItemFoodItem(mifiQueryDto);

            if (!creation_res) {
                throw new Error("Failed to create item.")
            }

            if (!creation_res.isSuccess) {
                throw new Error(`Fail - ${creation_res.error}`);
            }

            const retrieveUpdatedItems_res = await retrieveMenuItemFoodItem({
                menuItem_Id: null,
                foodItem_Id: null,
                consumption: null
            });

            if (!retrieveUpdatedItems_res) {
                throw new Error("Failed to retrieve updated items.");
            }

            if (!retrieveUpdatedItems_res.isSuccess) {
                throw new Error(`Fail - ${retrieveUpdatedItems_res.error}`);
            }

            setMenuItemFoodItem(retrieveUpdatedItems_res);
            setMifiQueryDto({
                menuItem_Id: null,
                foodItem_Id: null,
                consumption: null
            });
            onClose();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const retrieveItems = async () => {
        const responses = await Promise.all([retrieveIngredient({
            id      : null,
            unit_Id : null,
            type_Id : null,
            name    : null,
            quantity: null
        }), retrieveMenuItem({
            id  : null,
            name: null
        })]);

        responses.map(res => {
            if (!res) {
                throw new Error(`Failed to retrieve items.`);
            }

            if (!res.isSuccess) {
                throw new Error(`Fail - ${res.error}`);
            }

            if (isBasicDtoMenuItemDto(res)) {
                setMenuItem((res as BasicDto<MenuItemDto>));
            }

            if (isBasicDtoIngredientDto(res)) {
                setFoodItem((res as BasicDto<IngredientDto>));
            }
        });
    }

    function isMenuItemDto(obj: any): obj is MenuItemDto {
        return obj && typeof obj === 'object' && 'name' in obj; // Assuming MenuItemDto has a 'name' property
    }

    function isIngredientDto(obj: any): obj is IngredientDto {
        return obj && typeof obj === 'object' && 'quantity' in obj; // Assuming IngredientDto has a 'quantity' property
    }

    function isBasicDtoMenuItemDto(obj: any): obj is BasicDto<MenuItemDto> {
        return obj && obj.value && Array.isArray(obj.value.resultDto) && obj.value.resultDto.length > 0 && isMenuItemDto(obj.value.resultDto[0]);
    }

    function isBasicDtoIngredientDto(obj: any): obj is BasicDto<IngredientDto> {
        return obj && obj.value && Array.isArray(obj.value.resultDto) && obj.value.resultDto.length > 0 && isIngredientDto(obj.value.resultDto[0]);
    }


    useEffect(() => {
        (async () => {
            const menuItemFoodItem_res = await retrieveMenuItemFoodItem({
                menuItem_Id: null,
                foodItem_Id: null,
                consumption: null
            });

            if (!menuItemFoodItem_res) {
                throw new Error("Failed to retrieve Menu Item and Food Item.");
            }

            if (!menuItemFoodItem_res.isSuccess) {
                throw new Error(`Fail - ${menuItemFoodItem_res.error}`);
            }

            setMenuItemFoodItem(menuItemFoodItem_res);
            setIsLoading(false);

            retrieveItems().then();

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
                    {/*<Select*/}
                    {/*    label={"Food Item"}*/}
                    {/*    selectionMode={"single"}*/}
                    {/*    selectedKeys={editObj.foodItem_Id === 0*/}
                    {/*                  ? ""*/}
                    {/*                  : editObj.foodItem_Id.toString()}*/}
                    {/*    onSelectionChange={(selection) => updateFoodItem(Array.from(selection)[0] as number)}*/}
                    {/*>*/}
                    {/*    {*/}
                    {/*        foodItem.value.resultDto.map(value =>*/}
                    {/*            <SelectItem*/}
                    {/*                key={value.id}*/}
                    {/*                value={value.id}*/}
                    {/*                textValue={value.name}*/}
                    {/*            >*/}
                    {/*                {value.name}*/}
                    {/*            </SelectItem>*/}
                    {/*        )*/}
                    {/*    }*/}
                    {/*</Select>*/}
                    <Input
                        label={"Consumption"}
                        type={"number"}
                        min={1}
                        isRequired={true}
                        isInvalid={invalidConsumption}
                        defaultValue={editObj.consumption?.toString()}
                        onChange={(event) => updateConsumption(parseInt(event.target.value))}
                    />
                </>
            )
        } else {
            return (
                <>
                    <Select
                        label={"Menu Item"}
                        selectionMode={"single"}
                        value={mifiQueryDto.menuItem_Id
                               ? mifiQueryDto.menuItem_Id.toString()
                               : ""}
                        onSelectionChange={(selection) => createMenuItem(Array.from(selection)[0] as number)}
                    >
                        {
                            menuItem.value.resultDto.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.id}
                                    textValue={value.name}
                                >
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                    <Select
                        label={"Food Item"}
                        selectionMode={"single"}
                        value={mifiQueryDto.foodItem_Id
                               ? mifiQueryDto.foodItem_Id.toString()
                               : ""}
                        onSelectionChange={(selection) => createFoodItem(Array.from(selection)[0] as number)}
                    >
                        {
                            foodItem.value.resultDto.map(value =>
                                <SelectItem
                                    key={value.id}
                                    value={value.id}
                                    textValue={value.name}
                                >
                                    {value.name}
                                </SelectItem>
                            )
                        }
                    </Select>
                    <Input
                        label={"Consumption"}
                        type={"number"}
                        min={1}
                        isRequired={true}
                        isInvalid={invalidConsumption}
                        value={mifiQueryDto.consumption
                               ? mifiQueryDto.consumption?.toString()
                               : ""}
                        onChange={(event) => createConsumption(parseInt(event.target.value))}
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
                        menuItemFoodItem.value.resultDto.map((dto) =>
                            <TableRow key={dto.menuItem.name}>
                                <TableCell>{dto.menuItem.name}</TableCell>
                                <TableCell>{dto.foodItem.name}</TableCell>
                                <TableCell>{dto.consumption}</TableCell>
                                <TableCell width={"30px"}>
                                    <Button
                                        size={"sm"}
                                        onClick={() => handleDelete(dto.menuItem_Id, dto.foodItem_Id)}
                                    >
                                        <FontAwesomeIcon icon={faTrash} />
                                    </Button>
                                </TableCell>
                                <TableCell width={"30px"}>
                                    <Button
                                        size={"sm"}
                                        onClick={() => handleEdit(dto.menuItem_Id, dto.foodItem_Id)}
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
                        menuItemFoodItem.value.resultDto.map((dto) =>
                            <TableRow key={dto.menuItem.name}>
                                <TableCell>{dto.menuItem.name}</TableCell>
                                <TableCell>{dto.foodItem.name}</TableCell>
                                <TableCell>{dto.consumption}</TableCell>
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
                aria-label={"Stock"}
                topContent={<h1 className={"w-full text-center"}>Stock Management</h1>}
            >
                <TableHeader>
                    {
                        user?.role.includes("Manager")
                        ?
                        mifiHeaders.map(tableHeader => <TableColumn
                            key={tableHeader.key}
                        >{tableHeader.label}</TableColumn>)
                        : mifiHeadersStaff.map(tableHeader => <TableColumn
                            key={tableHeader.key}
                        >{tableHeader.label}</TableColumn>)
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
                <p>Are you sure you want to delete this order?</p>
            </Modals>
        </>
    );
}