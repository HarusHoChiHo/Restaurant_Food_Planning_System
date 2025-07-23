import React from 'react';
import { render, screen } from '@testing-library/react';
import FooterComponent from '../Footer';

describe('FooterComponent', () => {
  it('renders the footer with correct text', () => {
    render(<FooterComponent />);
    expect(screen.getByText(/Produced by Harus Ho, Kathy Tang/i)).toBeInTheDocument();
  });
}); 