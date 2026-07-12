export interface TaskImprovementRequest {
  originalTitle: string;
  originalDescription: string;
  additionalContext?: string;
  options: ImprovementOptions;
}

export interface ImprovementOptions {
  correctGrammar: boolean;
  improveClarity: boolean;
  makeProfessional: boolean;
  expandDescription: boolean;
  makeActionable: boolean;
  maxLength: number;
  tone: string;
  language?: string;
}

export interface TaskImprovementResponse {
  improvedTitle: string;
  improvedDescription: string;
  summary: string;
  keyPoints: string[];
  suggestedActions: string[];
  metadata: ImprovementMetadata;
}

export interface ImprovementMetadata {
  model: string;
  originalLength: number;
  improvedLength: number;
  processingTimeSeconds: number;
  tokensUsed: number;
  processedAt: Date;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[] | null;
  statusCode: number;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}