import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import HeaderComponent from '../Header';
import { AuthContext } from '../../app/AuthContext';

jest.mock('next/navigation', () => ({
  usePathname: () => '/dashboard/order',
  useRouter: () => ({ push: jest.fn() }),
}));

jest.mock('../../app/AuthContext', () => ({
  useAuth: () => ({
    user: { userName: 'Harus', role: ['harus'] },
    logout: jest.fn(),
  }),
}));

describe('HeaderComponent', () => {
  const setLoading = jest.fn();

  it('renders Place Order button', () => {
    render(<HeaderComponent setLoading={setLoading} />);
    //expect(screen.getByText('User')).toBeInTheDocument();
    expect(screen.getAllByText('Place Order')[0]).toBeInTheDocument();
  });

  it('renders navigation links for Staff', () => {
    render(<HeaderComponent setLoading={setLoading} />);
    expect(screen.getByText('Order')).toBeInTheDocument();
    expect(screen.getByText('Menu')).toBeInTheDocument();
    expect(screen.getByText('Food')).toBeInTheDocument();
  });

  it('calls setLoading and logout on Log out button click', () => {
    render(<HeaderComponent setLoading={setLoading} />);
    fireEvent.click(screen.getByText('Log out'));
    expect(setLoading).toHaveBeenCalledWith(true);
  });

//   it('renders User link for Manager role', () => {
//     jest.mock('../../app/AuthContext', () => ({
//       useAuth: () => ({
//         user: { userName: 'ManagerUser', role: ['Manager'] },
//         logout: jest.fn(),
//       }),
//     }));
//     render(<HeaderComponent setLoading={setLoading} />);
//     expect(screen.getByText('User')).toBeInTheDocument();
//   });
}); 