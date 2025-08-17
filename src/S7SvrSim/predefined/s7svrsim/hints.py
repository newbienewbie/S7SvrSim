from abc import ABC


class S7DB(ABC):
    pass
    

class S7MB(ABC):

    pass

class Logger(ABC):

    pass

class CancellationToken(ABC):
    pass

class S7Context(ABC):
    DBService: S7DB 
    MB: S7MB
    Logger: Logger
    CancellationToken: CancellationToken
    