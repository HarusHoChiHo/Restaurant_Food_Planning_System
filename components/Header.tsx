"use client"

import {
    Button,
    Divider,
    Dropdown,
    DropdownItem,
    DropdownMenu,
    DropdownTrigger,
    Navbar,
    NavbarBrand,
    NavbarContent,
    NavbarItem
} from "@nextui-org/react";
import Link from "next/link";
import React, {useEffect, useState} from "react";
import {usePathname, useRouter} from "next/navigation";
import {useAuth} from "../app/AuthContext";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faCaretDown, faDoorOpen, faPenNib} from "@fortawesome/free-solid-svg-icons";
import {Image} from "@nextui-org/image";

interface HeaderProps {
    setLoading: (loading : boolean) => void;
}

const HeaderComponent = ({setLoading} : HeaderProps) => {
    const [activeItem, setActiveItem] = useState("");
    const pathName = usePathname();
    const router = useRouter();
    const {
        user,
        logout
    } = useAuth();

    useEffect(() => {
        setActiveItem(pathName.split("/").pop() ?? "");
    }, [pathName]);

    const handleLogOut = () => {
        setLoading(true);
        logout();
        router.push("/");
    }

    const handlePlaceOrder = () => {
        router.push("/place-order");
    }

    const generateNavBarContent = () => {

        if (!user?.role.includes("Manager")) {
            return (
                <>
                    <NavbarContent
                        className={"flex w-full"}
                        justify={"center"}
                    >
                        <NavbarItem isActive={activeItem === "order"}>
                            <Link
                                href={"/dashboard/order"}
                                color={"foreground"}
                            >
                                Order
                            </Link>
                        </NavbarItem>
                        <Divider orientation="vertical" />
                        <Dropdown>
                            <NavbarItem>
                                <DropdownTrigger>
                                    <Button
                                        disableRipple
                                        endContent={<FontAwesomeIcon icon={faCaretDown} />}
                                        className="p-0 bg-transparent data-[hover=true]:bg-transparent"
                                        radius={"sm"}
                                        variant={"light"}
                                    >
                                        Menu
                                    </Button>
                                </DropdownTrigger>
                            </NavbarItem>
                            <DropdownMenu
                                selectionMode={"single"}
                                selectedKeys={[activeItem]}
                            >
                                <DropdownItem
                                    key={"menu"}
                                    href={"/dashboard/menu"}
                                >
                                    Menu Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"menu-item"}
                                    href={"/dashboard/menu-item"}
                                >
                                    Course Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"menu-item-food-item"}
                                    href={"/dashboard/menu-item-food-item"}
                                >
                                    Stock Management
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                        <Divider orientation="vertical" />
                        <Dropdown>
                            <NavbarItem>
                                <DropdownTrigger>
                                    <Button
                                        disableRipple
                                        endContent={<FontAwesomeIcon icon={faCaretDown} />}
                                        className="p-0 bg-transparent data-[hover=true]:bg-transparent"
                                        radius={"sm"}
                                        variant={"light"}
                                    >
                                        Food
                                    </Button>
                                </DropdownTrigger>
                            </NavbarItem>
                            <DropdownMenu
                                selectionMode={"single"}
                                selectedKeys={[activeItem]}
                            >
                                <DropdownItem
                                    key={"ingredient"}
                                    href={"/dashboard/ingredient"}
                                >
                                    Food Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"unit"}
                                    href={"/dashboard/unit"}
                                >
                                    Unit Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"type"}
                                    href={"/dashboard/type"}
                                >
                                    Type Management
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                    </NavbarContent>
                </>
            );
        } else {
            return (
                <>
                    <NavbarContent
                        className={"flex w-full"}
                        justify={"center"}
                    >
                        <NavbarItem isActive={activeItem === "order"}>
                            <Link
                                href={"/dashboard/order"}
                                color={"foreground"}
                            >
                                Order
                            </Link>
                        </NavbarItem>
                        <Divider orientation="vertical" />
                        <Dropdown>
                            <NavbarItem>
                                <DropdownTrigger>
                                    <Button
                                        disableRipple
                                        endContent={<FontAwesomeIcon icon={faCaretDown} />}
                                        className="p-0 bg-transparent data-[hover=true]:bg-transparent"
                                        radius={"sm"}
                                        variant={"light"}
                                    >
                                        Menu
                                    </Button>
                                </DropdownTrigger>
                            </NavbarItem>
                            <DropdownMenu
                                selectionMode={"single"}
                                selectedKeys={[activeItem]}
                            >
                                <DropdownItem
                                    key={"menu"}
                                    href={"/dashboard/menu"}
                                >
                                    Menu Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"menu-item"}
                                    href={"/dashboard/menu-item"}
                                >
                                    Course Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"menu-item-food-item"}
                                    href={"/dashboard/menu-item-food-item"}
                                >
                                    Stock Management
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                        <Divider orientation="vertical" />
                        <Dropdown>
                            <NavbarItem>
                                <DropdownTrigger>
                                    <Button
                                        disableRipple
                                        endContent={<FontAwesomeIcon icon={faCaretDown} />}
                                        className="p-0 bg-transparent data-[hover=true]:bg-transparent"
                                        radius={"sm"}
                                        variant={"light"}
                                    >
                                        Food
                                    </Button>
                                </DropdownTrigger>
                            </NavbarItem>
                            <DropdownMenu
                                selectionMode={"single"}
                                selectedKeys={[activeItem]}
                            >
                                <DropdownItem
                                    key={"ingredient"}
                                    href={"/dashboard/ingredient"}
                                >
                                    Food Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"unit"}
                                    href={"/dashboard/unit"}
                                >
                                    Unit Management
                                </DropdownItem>
                                <DropdownItem
                                    key={"type"}
                                    href={"/dashboard/type"}
                                >
                                    Type Management
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                        <Divider orientation="vertical" />
                        <NavbarItem isActive={activeItem === "user"}>
                            <Link
                                href={"/dashboard/user"}
                                color={"foreground"}
                            >
                                User
                            </Link>
                        </NavbarItem>
                    </NavbarContent>
                </>
            )
        }
    }

    const generateNavBarBrand = () => {
        return (
            <NavbarBrand className={"flex-col pt-4 pb-4"}>
                <span className="font-bold text-inherit underline">{user?.userName}</span>
                <Button
                    endContent={<FontAwesomeIcon icon={faDoorOpen} />}
                    className={"w-[125px] h-[25px] bg-gradient-to-tr from-pink-500 to-yellow-500 text-white shadow-lg"}
                    onPress={handleLogOut}
                >
                    Log out
                </Button>
                {
                    // pathName.includes("place-order") ?
                    //     (
                    //         <Button
                    //             href={"/dashboard"}
                    //             as={Link}
                    //             className={"mt-1 w-[125px] h-[25px] bg-gradient-to-tr from-pink-500 to-yellow-500 text-white shadow-lg"}
                    //         >
                    //             Go to Dashboard
                    //         </Button>
                    //     ) :
                    //     (
                    //         <Button endContent={<FontAwesomeIcon icon={faPenNib}/>}
                    //                 onPress={handlePlaceOrder}
                    //                 className={"mt-1 w-[125px] h-[25px] bg-gradient-to-tr from-pink-500 to-yellow-500 text-white shadow-lg"}
                    //         >
                    //             Place Order
                    //         </Button>
                    //     )
                }
                <Button
                    endContent={<FontAwesomeIcon icon={faPenNib} />}
                    onPress={handlePlaceOrder}
                    className={"mt-1 w-[125px] h-[25px] bg-gradient-to-tr from-pink-500 to-yellow-500 text-white shadow-lg"}
                >
                    Place Order
                </Button>
            </NavbarBrand>
        );
    }

    return (
        <Navbar
            position={"static"}
            isBordered={true}
            maxWidth={"full"}
            height={"max-content"}
            className={"m-0 p-0"}
        >
            <Image
                src={"/MenuMaster.png"}
                width={"110px"}
                height={"110px"}
            />
            {
                generateNavBarContent()
            }
            {
                generateNavBarBrand()
            }
        </Navbar>
    );
}

export default HeaderComponent;