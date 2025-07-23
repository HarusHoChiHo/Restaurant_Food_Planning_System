import React from 'react';
import { render, screen } from '@testing-library/react';
import ToastProvider from '../ToastAlert';

describe('ToastProvider', () => {
  it('renders children and ToastContainer', () => {
    render(
      <ToastProvider>
        <div>Test Child</div>
      </ToastProvider>
    );
    expect(screen.getByText('Test Child')).toBeInTheDocument();
    // ToastContainer renders a div with class 'Toastify__toast-container'
    expect(document.querySelector('.Toastify')).toBeInTheDocument();
  });
}); 