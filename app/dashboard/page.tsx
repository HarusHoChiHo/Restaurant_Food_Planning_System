"use client"

import {useAuth} from "../AuthContext";
import {Suspense} from "react";
import {Card, CardBody} from "@nextui-org/react";
import Loading from "../loading";

export default function Page() {
    const {
        user
    } = useAuth();
    
    return (
      <>
          <Suspense fallback={<Loading />}>
              <h2 className={"text-[48px]"}>Hello, {user?.userName}!</h2>
              <Card className={"w-1/2 h-[114px] flex flex-col justify-center items-center"}>
                  <CardBody className={"w-[272px] h-[125px] flex flex-col justify-center items-center"}>
                      <span className={"w-full"}>User name: {user?.userName}</span>
                      <span className={"w-full"}>Role: {user?.role.join(", ")}</span>
                  </CardBody>
              </Card>
          </Suspense>
      </>  
    );
}