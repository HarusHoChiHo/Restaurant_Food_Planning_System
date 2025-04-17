import {Button, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader} from "@nextui-org/react";
import React from "react";

interface ModalProps {
    isOpen: boolean;
    onOpenChange: (() => void) | ((isOpen: boolean) => void);
    onCancel: () => void;
    onConfirm: () => void | ((obj: any) => void);
    hideCloseButton: boolean;
    header: string;
    isDisabled?: boolean;
    children: React.ReactNode;
}

export default function Modals({
                                   isOpen,
                                   onOpenChange,
                                   onCancel,
                                   onConfirm,
                                   hideCloseButton,
                                   header,
                                   isDisabled = false,
                                   children
                               }: ModalProps) {


    return (
        <Modal isOpen={isOpen}
               onOpenChange={onOpenChange}
               hideCloseButton={hideCloseButton}
        >
            <ModalContent>
                {
                    <>
                        <ModalHeader className="flex flex-col gap-1">{header}</ModalHeader>
                        <ModalBody>
                            {children}
                        </ModalBody>
                        <ModalFooter>
                            <Button color="danger"
                                    variant="light"
                                    onPress={onCancel}>
                                Cancel
                            </Button>
                            <Button color="primary"
                                    onPress={onConfirm}
                                    isDisabled={isDisabled}
                            >
                                Confirm
                            </Button>
                        </ModalFooter>
                    </>
                }
            </ModalContent>
        </Modal>
    )
}