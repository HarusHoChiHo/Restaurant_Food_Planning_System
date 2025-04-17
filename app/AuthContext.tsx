'use client'
import React, {createContext, useState, useEffect, ReactNode} from "react";
import HttpServices from "../lib/HttpServices";
import TokenDto from "../lib/models/TokenDto";
import UserDto from "../lib/models/user/UserDto";
import UserInfo from "../lib/models/user/UserInfo";
import {toast} from "react-toastify";

interface AuthContextType {
    token: string | null;
    user: UserInfo | null;
    login: (token: string, user: UserDto) => void;
    logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: ReactNode;
}

export function AuthProvider({children}: AuthProviderProps) {
    const [token, setToken] = useState<string | null>(null);
    const [user, setUser] = useState<UserInfo | null>(null);
    const httpServices = new HttpServices();
    const [loading, setLoading] = useState(true);

    const showToast = (message: string) => {
        toast(message);
    }
    
    useEffect(() => {
        const storedToken = localStorage.getItem("token");
        const storedUser = localStorage.getItem("user");
        (async () => {
            setLoading(true);
            if (storedToken && storedUser) {
                try {
                    let server_res = await (await httpServices.callAPI("/TokenValidation", {token: storedToken}, "POST")).json() as BasicDto<TokenDto>;
                    if (server_res.value.resultDto.pop()?.valid) {
                        setToken(storedToken);
                        setUser(JSON.parse(storedUser));
                        setLoading(false);
                    } else {
                        localStorage.removeItem("token");
                        localStorage.removeItem("user");
                        setLoading(false);
                    }
                } catch (error){
                    if (error instanceof Error) {
                        showToast(error.message);
                    } else {
                        showToast("Service crashed")
                    }
                }
            } else {
                setLoading(false);
            }
        })();
    }, []);

    const login = (newToken: string, user: UserDto) => {
        setToken(newToken);
        setUser(user);
        const {userName, role} = user;
        const userInfo = new UserInfo(userName, role);
        localStorage.setItem("token", newToken);
        localStorage.setItem("user", JSON.stringify(userInfo));
    }

    const logout = () => {
        setToken(null);
        setUser(null);
        localStorage.removeItem("token");
        localStorage.removeItem("user");
    }

    const value = {
        token,
        user,
        login,
        logout
    }

    return (
        <AuthContext.Provider value={value}>
            {!loading && children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextType {
    const context = React.useContext(AuthContext);

    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }

    return context;
}

