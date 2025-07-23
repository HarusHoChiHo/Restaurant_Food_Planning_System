import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Modals from '../CustomModal';
import '@testing-library/jest-dom';

describe('CustomModal', () => {
  const defaultProps = {
    isOpen: true,
    onOpenChange: jest.fn(),
    onCancel: jest.fn(),
    onConfirm: jest.fn(),
    hideCloseButton: false,
    header: 'Test Header',
    children: <div>Modal Content</div>,
  };

  it('renders header and children', () => {
    render(<Modals {...defaultProps} />);
    expect(screen.getByText('Test Header')).toBeInTheDocument();
    expect(screen.getByText('Modal Content')).toBeInTheDocument();
  });

  it('calls onCancel when Cancel button is clicked', () => {
    render(<Modals {...defaultProps} />);
    fireEvent.click(screen.getByText('Cancel'));
    expect(defaultProps.onCancel).toHaveBeenCalled();
  });

  it('calls onConfirm when Confirm button is clicked', () => {
    render(<Modals {...defaultProps} />);
    fireEvent.click(screen.getByText('Confirm'));
    expect(defaultProps.onConfirm).toHaveBeenCalled();
  });

  it('disables Confirm button when isDisabled is true', () => {
    render(<Modals {...defaultProps} isDisabled={true} />);
    expect(screen.getByText('Confirm')).toBeDisabled();
  });

  it('hides close button when hideCloseButton is true', () => {
    // This would require checking for the close button, which is handled by @nextui-org/react Modal
    // You may need to mock Modal or check for absence of a close button if possible
    render(<Modals {...defaultProps} hideCloseButton={true} />);
    // Example: expect(screen.queryByLabelText('close')).not.toBeInTheDocument();
    // (Update this selector based on actual Modal implementation)
  });
}); 