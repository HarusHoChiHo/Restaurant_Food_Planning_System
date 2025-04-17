'use client'

import React, {useEffect, useState} from "react";
import {NextPage} from "next";
import HttpServices from "../../lib/HttpServices";
import {useRouter} from "next/navigation";
import LoginQueryDto from "../../lib/models/LoginQueryDto";
import {useAuth} from "../AuthContext";
import UserDto from "../../lib/models/user/UserDto";
import {Image} from "@nextui-org/image";
import {Input} from "@nextui-org/react";
import {toast} from "react-toastify";

interface LoginProps {
    closeLoginPage: (state: boolean) => void;
}

const LoginComponent = ({closeLoginPage}: LoginProps) => {
    const router = useRouter();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [hidden, setHidden] = useState(true);
    const api = new HttpServices();
    const {
        login
    } = useAuth();

    const handleLogin = (e: React.FormEvent) => {
        e.preventDefault();
        api.callAPI("/user/login", new LoginQueryDto(username, password), "POST").then(r => {
            if (r.status === 200) {
                r.json().then(res => {
                    login(res.value.token, (res as LoginDto<UserDto>).value.resultDto[0]);
                    closeLoginPage(false);
                    router.refresh();
                }).catch(error => {
                    if (error instanceof Error) {
                        showToast(error.message);
                    } else {
                        showToast("Service crashed");
                    }
                });
            } else {
                setHidden(false);
            }
        });
    }

    const showToast = (message: string) => {
        toast(message);
    }
    
    return (
            <div className={"w-[95%] h-[90%] flex flex-col justify-center items-center bg-gray-100"}>
                <Image src={"/MenuMaster.png"}
                       width={"60px"}
                       height={"60px"}
                       className={"mt-[43px]"}
                />
                <h2 className={"mt-4"}>Restaurant Food Planning System</h2>
                <div className={"flex flex-col items-center size-full mt-[65px]"}>
                    <h2 className={"text-center mb-[55px] w-[125px] h-[58px] text-[48px]"}>Login</h2>
                    <Input type={"text"}
                           placeholder={"Enter your username"}
                           value={username}
                           onChange={(e) => setUsername(e.target.value)}
                           className={"mb-[7px] w-[300px] h-[40px]"}
                           required={true}
                    />
                    <Input type={"password"}
                           placeholder={"Enter your password"}
                           value={password}
                           onChange={(e) => setPassword(e.target.value)}
                           className={"mb-[40px] w-[300px] h-[40px]"}
                           required={true}
                    />
                    <button type={"submit"}
                            color={"primary"}
                            className={"w-[200px] h-[40px] border-2 text-white bg-black"}
                            onClick={handleLogin}
                    >SIGN IN
                    </button>
                    <button
                        color={"primary"}
                        className={"w-[200px] h-[40px] border-2 text-white bg-black"}
                        onClick={() => closeLoginPage(false)}
                    >
                        Cancel
                    </button>
                    <p hidden={hidden}>Invalid Username and password.</p>
                </div>
            </div>
    );
}

export default LoginComponent;