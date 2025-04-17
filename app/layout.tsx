import type {Metadata} from "next";
import {Inter} from "next/font/google";
import "./globals.css";
import {library} from '@fortawesome/fontawesome-svg-core'
import {fas} from '@fortawesome/free-solid-svg-icons'
import {fab} from '@fortawesome/free-brands-svg-icons'
import '@fortawesome/fontawesome-svg-core/styles.css';
import {Providers} from "./providers";
import React from "react";

library.add(fas, fab);

const inter = Inter({subsets: ["latin"]});

export const metadata: Metadata = {
    title      : "Restaurant Food Planning System",
    description: "Manage and plan your food",
};

export default function RootLayout({
                                       children,
                                   }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
        <body className={`${inter.className} bg-gradient-to-r from-emerald-300 to-indigo-400 size-full min-h-dvh box-border`}>
        <Providers>
            {children}
        </Providers>
        </body>
        </html>
    );
}
