"use client"

import {NextUIProvider} from '@nextui-org/react'
import {AuthProvider} from "./AuthContext";
import ToastProvider from "../components/ToastAlert";

export function Providers({children}: {
    children: React.ReactNode
}) {
    return (
        <ToastProvider>
            <AuthProvider>
                <NextUIProvider>
                    {children}
                </NextUIProvider>
            </AuthProvider>
        </ToastProvider>
    )
}