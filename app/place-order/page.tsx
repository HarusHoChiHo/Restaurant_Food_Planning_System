"use client"

import HttpServices from "../../lib/HttpServices";
import {useAuth} from "../AuthContext";
import React, {useEffect, useState} from "react";
import OrderPlacementQueryDto from "../../lib/models/order/OrderPlacementQueryDto";
import OrderPlacementDto from "../../lib/models/order/OrderPlacementDto";
import {Button, Card, CardBody, CardFooter, CardHeader, Divider, Select, SelectItem, Spinner} from "@nextui-org/react";
import OrderItemQueryDto from "../../lib/models/order/OrderItemQueryDto";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faCheck} from "@fortawesome/free-solid-svg-icons";
import {useRouter} from "next/navigation";
import FooterComponent from "../../components/Footer";
import HeaderComponent from "../../components/Header";
import {toast} from "react-toastify";
import MenuQueryDto from "../../lib/models/menu/MenuQueryDto";
import MenuDto from "../../lib/models/menu/MenuDto";

export default function OrderComponent() {
    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const router = useRouter();
    const [menu, setMenu] = useState<BasicDto<MenuDto>>({
        error    : "",
        isSuccess: false,
        value    : {
            amount   : 0,
            resultDto: [{
                date    : new Date(),
                id      : 0,
                menuItem: {
                    id  : 0,
                    name: ""
                }
            }]
        }
    });
    const [selectedItems, setSelectedItems] = useState<string[]>([]);
    const [addedOrderItems, setAddedOrderItems] = useState<string[]>([])
    const [valid, setValid] = useState(true);
    const [isLoading, setIsLoading] = useState(true);
    const menuAPI: string = "/DataManagement/menu";
    const orderPlacementAPI: string = "/Order";


    const [orderPlacementDto, setOrderPlacementDto] = useState<OrderPlacementQueryDto>()

    const retrieveMenu = async function (menuQuery: MenuQueryDto) {
        try {
            let response = await (await httpServices.callAPI(`${menuAPI}/read`, menuQuery, "POST", token)).json();
            return response as BasicDto<MenuDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const createOrder = async function () {
        try {
            let response = await (await httpServices.callAPI(`${orderPlacementAPI}/place-order`, orderPlacementDto, "POST", token)).json();
            return response as BasicDto<OrderPlacementDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        }
    }

    const updateOrder = function (selectedMenuItemId: string[]) {
        setSelectedItems(selectedMenuItemId);
        setValid(true);
    }
    
    const addSelectedOrderItem = function () {
        setAddedOrderItems([...addedOrderItems,...selectedItems]);
        setSelectedItems([]);
        setOrderPlacementDto({
            order     : {
                id        : null,
                isCanceled: false
            },
            orderItems: addedOrderItems.map(id => new OrderItemQueryDto(null, null, parseInt(id)))
        });
    }

    const confirmOrder = () => {
        (async () => {
                if (!orderPlacementDto?.orderItems) {
                    setValid(false);
                    return;
                } else {
                    if (orderPlacementDto?.orderItems.length < 1) {
                        setValid(false);
                        return;
                    }
                }
                
                const createRes = await createOrder();

                if (!createRes) {
                    throw new Error("Failed to create place-order.");
                }

                if (!createRes.isSuccess) {
                    throw new Error(`Fail - ${createRes.error}`);
                }

                setSelectedItems([]);
                setOrderPlacementDto(undefined);
                setValid(true);
                showToast("Successfully create a new place-order");
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }

    const showToast = (message: string) => {
        toast(message);
    }
    
    const showLoadingPage = (loading: boolean) => {
        setIsLoading(loading);
    }
    
    useEffect(() => {
        if (!token || !user) {
            router.push("/login");
        }
        (async () => {
                const retrieveRes = await retrieveMenu({
                    id         : null,
                    date       : new Date().toDateString(),
                    menuItem_Id: null
                });

                if (!retrieveRes) {
                    throw new Error("Failed to retrieve menu items.");
                }

                if (!retrieveRes.isSuccess) {
                    throw new Error(`Fail - ${retrieveRes.error}`);
                }
                setMenu(retrieveRes);
                setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed")
            }
        });
    }, []);

    if (isLoading) {
        return (
            <>
                <div className={"min-h-dvh size-full flex flex-col items-center justify-center"}>
                    <Spinner/>
                </div>
            </>
        );
    }

    return <>
        <HeaderComponent setLoading={showLoadingPage}/>
        <div className={"w-full flex flex-col items-center"}>
            <h1 className={"m-4"}>Place your order!!!!</h1>
            <div className={"w-1/2"}>
                <Card>
                    <CardBody>
                        <Select label={"Course"}
                                selectionMode={"multiple"}
                                placeholder={menu.value.resultDto.length > 0 ? "Select your course" : "No item for today."}
                                selectedKeys={selectedItems}
                                isDisabled={!(menu.value.resultDto.length > 0)}
                                isRequired={true}
                                isInvalid={!valid}
                                errorMessage={"Must select a course"}
                                onSelectionChange={(key) => updateOrder(Array.from(key, opt => opt.toString()))}
                        >
                            {

                                menu.value.resultDto.map(mItem =>
                                    <SelectItem key={mItem.menuItem.id} textValue={mItem.menuItem.name}>
                                        {mItem.menuItem.name}
                                    </SelectItem>
                                )
                            }
                        </Select>
                        <Divider className={"mt-2 mb-2"}/>
                        <Button
                            size={"lg"}
                            variant={"solid"}
                            color={"primary"}
                            onPress={ () => addSelectedOrderItem()}
                        >
                            Add to bucket
                        </Button>
                    </CardBody>
                    <Divider className={"mt-2 mb-2"}/>
                    <CardBody>
                        <p className={"text-small text-default-500 p-4"}>Selected Courses:</p>
                        {
                            addedOrderItems.length > 0 ? <ol className={"list-decimal list-inside ps-4"}>
                                    {
                                        addedOrderItems.map((id, index) =>
                                            <li key={index}>
                                                {
                                                    menu.value.resultDto.filter(item => item.menuItem.id.toString() === id)[0].menuItem.name
                                                }
                                            </li>
                                        )
                                    }
                                </ol> : <p className={"text-small text-default-400 p-4"}>No any item is selected.</p>
                        }

                    </CardBody>
                    <Divider className={"mt-2 mb-2"}/>
                    <CardFooter className={"flex justify-end"}>
                        <Button
                            startContent={<FontAwesomeIcon icon={faCheck}/>}
                            size={"lg"}
                            variant={"solid"}
                            color={"primary"}
                            onPress={confirmOrder}
                        >
                            Confirm your order
                        </Button>
                    </CardFooter>
                </Card>
            </div>
        </div>
        <FooterComponent/>
    </>;
}