"use client"

import {useAuth} from "../AuthContext";
import React, {Suspense, useEffect, useState} from "react";
import {useRouter} from "next/navigation";
import HeaderComponent from "../../components/Header";
import FooterComponent from "../../components/Footer";
import Loading from "../loading";

const DashboardLayout = ({children}: {
    children: React.ReactNode
}) => {
    const {
        token,
        user
    } = useAuth();
    const router = useRouter();
    const [isLoading, setIsLoading] = useState(true);
    
    useEffect(() => {
        if (!token || !user) {
            router.push("/");
            return;
        }
        
        setIsLoading(false);
    }, [router]);

    const showLoadingPage = (loading: boolean) => {
        setIsLoading(loading);
    }
    
    return (
        <div>
            {
                (
                    <>
                        <Suspense fallback={<Loading />}>
                            <HeaderComponent setLoading={showLoadingPage} />
                            <div className={"flex flex-col justify-center items-center w-6/12 ml-auto mr-auto pt-8 pb-8"}>
                                {!isLoading && children}
                            </div>
                            <FooterComponent/>
                        </Suspense>
                    </>
                )
            }
        </div>);
}

export default DashboardLayout;