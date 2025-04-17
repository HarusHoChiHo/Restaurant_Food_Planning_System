"use client"

import HttpServices from "../../../lib/HttpServices";
import {useAuth} from "../../AuthContext";
import React, {Fragment, useEffect, useState} from "react";
import {
    Button,
    Checkbox,
    Pagination,
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
import OrderDto, {orderHeaders, orderHeadersStaff} from "../../../lib/models/order/OrderDto";
import {OrderQueryDto, OrderQueryPerPageDto} from "../../../lib/models/order/OrderQueryDto";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faTrash} from "@fortawesome/free-solid-svg-icons/faTrash";
import {faPenToSquare} from "@fortawesome/free-solid-svg-icons/faPenToSquare";
import Modals from "../../../components/CustomModal";
import OrderItemDto from "../../../lib/models/order/OrderItemDto";
import {toast} from "react-toastify";

export default function OrderComponent() {
    const httpServices = new HttpServices();
    const {
        token,
        user
    } = useAuth();
    const orderAPI: string = "/DataManagement/order";
    const orderItemAPI: string = "/DataManagement/order-item";
    const {
        isOpen,
        onOpen,
        onClose,
        onOpenChange
    } = useDisclosure();

    const [editObj, setEditObj] = useState<OrderDto>(
        {
            id        : 0,
            isCanceled: false,
            orderItems: []
        }
    );

    const [order, setOrder] = useState<BasicDto<OrderDto>>(
        {
            error    : "",
            isSuccess: false,
            value    : {
                amount   : 0,
                resultDto: [{
                    id        : 0,
                    isCanceled: false,
                    orderItems: [{
                        id      : 0,
                        orderId : 0,
                        menuItem: {
                            id  : 0,
                            name: ""
                        }
                    }]
                }]
            }
        });
    const [isCanceled, setIsCanceled] = useState("false");
    const [isLoading, setIsLoading] = useState(true);
    const [orderItemList, setOrderItemList] = useState<number[]>([]);
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState(1);
    const [displayOrders, setDisplayOrders] = useState<OrderDto[]>([]);
    const [orderPerPage, setOrderPerPage] = useState<Map<number, OrderDto[]>>(new Map());
    const [isDeletionModalOpen, setDeletionModalOpen] = useState(false);
    const [currentIdForDeletion, setCurrentIdForDeletion] = useState<number | null>(null);
    const itemsPerPage = 5;
    
    const handleDelete = (id: number) => {
        setCurrentIdForDeletion(id);
        setDeletionModalOpen(true);
    }

    const confirmDelete = () => {
        (async () => {
            if(!currentIdForDeletion){
                throw new Error("The current id for deletion is null");
            }
            const serverRes = await deleteOrder(currentIdForDeletion);

            if (!serverRes) {
                throw new Error("Failed to retrieve place-order.");
            }

            if (!serverRes.isSuccess) {
                throw new Error(`Fail - ${serverRes.error}`);
            }

            const retrieveRes = await retrieveOrder({
                id        : null,
                isCanceled: null,
                pageNumber: currentPage,
                limit     : itemsPerPage
            });

            if (!retrieveRes) {
                throw new Error("Failed to retrieve updated place-order");
            }

            if (!retrieveRes.isSuccess) {
                throw new Error(`Fail - ${retrieveRes.error}`);
            }

            const newMap = new Map(orderPerPage);
            newMap.set(currentPage, retrieveRes.value.resultDto);
            setOrderPerPage(newMap);

            setOrder(retrieveRes);

            setEditObj(serverRes.value.resultDto[0]);
            setIsCanceled(`${serverRes.value.resultDto[0].isCanceled}`);
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
    
    const handlePageChange = (page: number) => {
        setCurrentPage(page);
    }
    
    const handleEdit = (id: number) => {
        (async () => {
            const serverRes = await retrieveOrder({
                id        : id,
                isCanceled: null,
                pageNumber: null,
                limit     : null
            });

            if (!serverRes) {
                throw new Error("Failed to retrieve place-order.");
            }

            if (!serverRes.isSuccess) {
                throw new Error(`Fail - ${serverRes.error}`);
            }
            
            setEditObj(serverRes.value.resultDto[0]);
            setIsCanceled(`${serverRes.value.resultDto[0].isCanceled}`);
            onOpen();
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const updateStatus = (status: string[]) => {
        setIsCanceled(status[0]);
    }

    const retrieveOrder = async (orderQueryDto: OrderQueryPerPageDto) => {
        try {
            const server_res = await (await httpServices.callAPI(`${orderAPI}/read`, orderQueryDto, "POST", token)).json();
            return server_res as BasicDto<OrderDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const deleteOrder = async (id: number) => {
        try {
            const server_res = await (await httpServices.callAPI(`${orderAPI}/${id}`, null, "DELETE", token)).json();
            return server_res as BasicDto<OrderDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const updateOrder = async (orderQueryDto: OrderQueryDto) => {
        try {
            const server_res = await (await httpServices.callAPI(`${orderAPI}/update`, orderQueryDto, "POST", token)).json();
            return server_res as BasicDto<OrderDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const deleteOrderItem = async (id: number) => {
        try {
            const server_res = await (await httpServices.callAPI(`${orderItemAPI}/${id}`, null, "DELETE", token)).json();
            return server_res as BasicDto<OrderItemDto>;
        } catch (error) {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        }
    }

    const cancelEdition = () => {
        onClose();
    }

    const confirmEdition = () => {
        (async () => {
            if (orderItemList) {
                const updateRes = await updateOrder({
                    id        : editObj.id,
                    isCanceled: (isCanceled === "true")
                });
                if (!updateRes) {
                    throw new Error("Failed to update place-order.");
                }

                if (!updateRes.isSuccess) {
                    throw new Error(`Fail - ${updateRes.error}`);
                }

                const promiseList = orderItemList.map(item => deleteOrderItem(item));
                const serverRes = await Promise.all(promiseList);

                const failedServerRes = serverRes.filter(res => !res!.isSuccess);

                if (failedServerRes.length > 0) {
                    throw new Error("Failed to update place-order item");
                }

                const retrieveServerRes = await retrieveOrder({
                    id        : null,
                    isCanceled: null,
                    pageNumber: currentPage,
                    limit     : itemsPerPage
                });

                if (!retrieveServerRes) {
                    throw new Error("Failed to retrieve update place-order");
                }

                if (!retrieveServerRes.isSuccess) {
                    throw new Error(`Fail - ${retrieveServerRes.error}`);
                }

                const newMap = new Map(orderPerPage);
                newMap.set(currentPage, retrieveServerRes.value.resultDto);
                setOrderPerPage(newMap);
                
                setOrder(retrieveServerRes);
                onClose();
            }
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }

    const updateOrderItemList = (checked: boolean, orderItemId: number) => {
        if (!checked) {
            setOrderItemList([...orderItemList, orderItemId]);
        } else {
            setOrderItemList(orderItemList.filter(id => id !== orderItemId));
        }
    }

    const showToast = (message: string) => {
        toast(message);
    }

    // useEffect(() => {
    //     (async () => {
    //         const retrieveResult = await retrieveOrder({
    //             id        : null,
    //             isCanceled: null,
    //             pageNumber: currentPage,
    //             limit     : itemsPerPage
    //         });
    //
    //         if (!retrieveResult) {
    //             throw new Error("Failed to retrieve place-order.");
    //         }
    //
    //         if (!retrieveResult.isSuccess) {
    //             throw new Error(`Fail - ${retrieveResult.error}`);
    //         }
    //
    //         setOrder(retrieveResult);
    //
    //         setIsLoading(false);
    //     })().catch(error => {
    //         if (error instanceof Error) {
    //             showToast(error.message);
    //         } else {
    //             showToast("Service crashed");
    //         }
    //     });
    // }, []);

    useEffect(() => {
        setTotalPages(Math.ceil(order.value.amount / itemsPerPage));
        const items = orderPerPage.get(currentPage);
        if (items){
            setDisplayOrders(items);
        }
        
    }, [order]);

    useEffect(() => {
        (async () => {
            const retrieveResult = await retrieveOrder({
                id        : null,
                isCanceled: null,
                pageNumber: currentPage,
                limit     : itemsPerPage
            });

            if (!retrieveResult) {
                throw new Error("Failed to retrieve place-order.");
            }

            if (!retrieveResult.isSuccess) {
                throw new Error(`Fail - ${retrieveResult.error}`);
            }
            
            const newMap = new Map(orderPerPage);
            newMap.set(currentPage, retrieveResult.value.resultDto);
            setOrderPerPage(newMap);
            
            setOrder(retrieveResult);

            setIsLoading(false);
        })().catch(error => {
            if (error instanceof Error) {
                showToast(error.message);
            } else {
                showToast("Service crashed");
            }
        });
    }, [currentPage]);
    
    const renderContent = () => {
        return (
            <>
                <Select
                    label={"Order Status"}
                    selectionMode={"single"}
                    selectedKeys={[isCanceled]}
                    onSelectionChange={(option) => updateStatus(Array.from(option, opt => opt.toString()))}
                >
                    <SelectItem
                        key={"false"}
                        value={"false"}
                    >
                        {"Not Canceled"}
                    </SelectItem>
                    <SelectItem
                        key={"true"}
                        value={"true"}
                    >
                        {"Canceled"}
                    </SelectItem>
                </Select>
                {
                    editObj.orderItems.map(iorder =>
                        <Fragment key={iorder.id}>
                            <div className={"flex flex-row w-full"}>
                                <Checkbox
                                    defaultSelected
                                    onValueChange={(checked) => updateOrderItemList(checked, iorder.id)}
                                >
                                    {iorder.menuItem.name}
                                </Checkbox>
                            </div>
                        </Fragment>
                    )
                }
            </>
        )
    }

    const generateOptionalFields = () => {
        if (user?.role.includes("Manager")) {
            return (
                <TableBody emptyContent={"No rows to display."}>
                    {
                        displayOrders.map((dto) =>
                            <TableRow key={dto.id}>
                                <TableCell>{dto.id}</TableCell>
                                <TableCell>{dto.isCanceled
                                            ? "Yes"
                                            : "No"}</TableCell>
                                <TableCell>
                                    {
                                        <ol>
                                            {
                                                dto.orderItems.map(item =>
                                                    <li key={item.id}>
                                                        {item.menuItem.name}
                                                    </li>
                                                )
                                            }
                                        </ol>
                                    }
                                </TableCell>
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
                        displayOrders.map((dto) =>
                            <TableRow key={dto.id}>
                                <TableCell>{dto.id}</TableCell>
                                <TableCell>{dto.isCanceled
                                            ? "Yes"
                                            : "No"}</TableCell>
                                <TableCell>
                                    {
                                        <ol>
                                            {
                                                dto.orderItems.map(item =>
                                                    <li key={item.id}>
                                                        {item.menuItem.name}
                                                    </li>
                                                )
                                            }
                                        </ol>
                                    }
                                </TableCell>
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
            <Table
                aria-label={"Order"}
                topContent={<h1 className={"w-full text-center"}>Order Management</h1>}
                bottomContent={
                    <div className={"flex w-full justify-center"}>
                        <Pagination
                            showControls={true}
                            showShadow={true}
                            isCompact={true}
                            total={totalPages}
                            initialPage={1}
                            page={currentPage}
                            onChange={handlePageChange}
                        />
                    </div>}
            >
                <TableHeader>
                    {
                        user?.role.includes("Manager")
                        ?
                        orderHeaders.map(tableHeader => <TableColumn
                            key={tableHeader.key}
                        >{tableHeader.label}</TableColumn>)
                        : orderHeadersStaff.map(tableHeader => <TableColumn
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
                onCancel={cancelEdition}
                onConfirm={confirmEdition}
                header={"Edit"}
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