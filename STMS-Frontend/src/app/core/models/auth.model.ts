export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  role: string;
  expiresAt: Date;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}